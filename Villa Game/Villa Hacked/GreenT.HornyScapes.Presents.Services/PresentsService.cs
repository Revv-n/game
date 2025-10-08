using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.Characters;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.Presents.Analytics;
using GreenT.HornyScapes.Presents.Models;
using GreenT.HornyScapes.Presents.UI;
using GreenT.HornyScapes.Relationships.Mappers;
using GreenT.HornyScapes.Relationships.Models;
using GreenT.HornyScapes.Relationships.Providers;
using GreenT.Types;
using Merge.Meta.RoomObjects;
using StripClub.Model;
using StripClub.Model.Cards;
using StripClub.Model.Shop;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Presents.Services;

public class PresentsService : IInitializable, IDisposable
{
	private const int ONE = 1;

	private CharacterInfo _characterInfo;

	private Relationship _relationship;

	private IPromote _promote;

	private ReactiveProperty<int> _currency;

	private CompositeIdentificator _relationshipId;

	private int _presentCount;

	private IDisposable _currencyStream;

	private readonly ICurrencyProcessor _currencyProcessor;

	private readonly PresentsViewTapTracker _presentsViewTapTracker;

	private readonly CardsCollection _cards;

	private readonly RelationshipMapperProvider _relationshipMapperProvider;

	private readonly RelationshipRewardMapperProvider _relationshipRewardMapperProvider;

	private readonly PresentsManager _presentsManager;

	private readonly PresentsAnalytic _presentsAnalytic;

	private readonly PresentsNotifier _presentsNotifier;

	private readonly WalletProvider _walletProvider;

	private readonly Currencies _currencies;

	private readonly GameStarter _gameStarter;

	private readonly Dictionary<int, List<int>> _rewardPromoteLevels = new Dictionary<int, List<int>>(64);

	private readonly Subject<PresentView> _spended = new Subject<PresentView>();

	private readonly Subject<(PresentView PresentView, PresentSpendFailReason Reason)> _spendFailed = new Subject<(PresentView, PresentSpendFailReason)>();

	private readonly CompositeDisposable _disposables = new CompositeDisposable();

	public IObservable<PresentView> Spended => (IObservable<PresentView>)_spended;

	public IObservable<(PresentView PresentView, PresentSpendFailReason Reason)> SpendFailed => (IObservable<(PresentView PresentView, PresentSpendFailReason Reason)>)_spendFailed;

	public PresentsService(ICurrencyProcessor currencyProcessor, PresentsViewTapTracker presentsViewTapTracker, CardsCollection cards, RelationshipMapperProvider relationshipMapperProvider, RelationshipRewardMapperProvider relationshipRewardMapperProvider, PresentsManager presentsManager, PresentsAnalytic presentsAnalytic, PresentsNotifier presentsNotifier, WalletProvider walletProvider, Currencies currencies, GameStarter gameStarter)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		_currencyProcessor = currencyProcessor;
		_presentsViewTapTracker = presentsViewTapTracker;
		_cards = cards;
		_relationshipMapperProvider = relationshipMapperProvider;
		_relationshipRewardMapperProvider = relationshipRewardMapperProvider;
		_presentsManager = presentsManager;
		_presentsAnalytic = presentsAnalytic;
		_presentsNotifier = presentsNotifier;
		_walletProvider = walletProvider;
		_currencies = currencies;
		_gameStarter = gameStarter;
	}

	public void Initialize()
	{
		_currencyStream = ObservableExtensions.Subscribe<bool>(Observable.Where<bool>((IObservable<bool>)_gameStarter.IsGameActive, (Func<bool, bool>)((bool x) => x)), (Action<bool>)delegate
		{
			SetCurrencies();
		});
	}

	public void Set(CharacterInfo characterInfo, Relationship relationship)
	{
		_characterInfo = characterInfo;
		_relationship = relationship;
		_relationshipId = new CompositeIdentificator(relationship.ID);
		_promote = _cards.GetPromoteOrDefault(_characterInfo);
		_currency = _currencies.Get(CurrencyType.LovePoints, _relationshipId);
		CheckRewardPromoteLevels();
		CompositeDisposable disposables = _disposables;
		if (disposables != null)
		{
			disposables.Clear();
		}
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<PresentView>(_presentsViewTapTracker.PresentSended, (Action<PresentView>)TrySendPresent), (ICollection<IDisposable>)_disposables);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<PresentView>(_presentsViewTapTracker.PressStarted, (Action<PresentView>)StartTrackPresent), (ICollection<IDisposable>)_disposables);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<PresentView>(_presentsViewTapTracker.PressEnded, (Action<PresentView>)delegate(PresentView presentView)
		{
			EndTrackPresent(presentView);
		}), (ICollection<IDisposable>)_disposables);
	}

	public void Dispose()
	{
		CompositeDisposable disposables = _disposables;
		if (disposables != null)
		{
			disposables.Dispose();
		}
		_currencyStream?.Dispose();
	}

	private void TrySendPresent(PresentView presentView)
	{
		Present source = presentView.Source;
		if (source.Container.Count <= 0)
		{
			_spendFailed?.OnNext((presentView, PresentSpendFailReason.NotEnought));
			EndTrackPresent(presentView);
			return;
		}
		if (_promote.Level.Value <= _relationship.RelationshipLevel.Value && IsPromoteBlocked())
		{
			_spendFailed?.OnNext((presentView, PresentSpendFailReason.PromoteBlocked));
			EndTrackPresent(presentView);
			return;
		}
		if (!CanSpend() || !source.AddCount(-1))
		{
			EndTrackPresent(presentView);
			return;
		}
		AddPointsByType(source);
		_presentsNotifier.Notify(source);
		_presentCount++;
		_spended?.OnNext(presentView);
	}

	private void AddPointsByType(Present present)
	{
		_currencyProcessor.TryAdd(CurrencyType.LovePoints, present.LovePoints, CurrencyAmplitudeAnalytic.SourceType.Relationship, _relationshipId);
	}

	private void CheckRewardPromoteLevels()
	{
		_rewardPromoteLevels.Clear();
		int[] rewards = _relationshipMapperProvider.Get(_relationship.ID).rewards;
		foreach (int id in rewards)
		{
			RelationshipRewardMapper relationshipRewardMapper = _relationshipRewardMapperProvider.Get(id);
			int promote_to_unlock = relationshipRewardMapper.promote_to_unlock;
			int points_to_unlock = relationshipRewardMapper.points_to_unlock;
			if (_rewardPromoteLevels.ContainsKey(promote_to_unlock))
			{
				_rewardPromoteLevels[promote_to_unlock].Add(points_to_unlock);
				continue;
			}
			_rewardPromoteLevels.Add(promote_to_unlock, new List<int>(8) { points_to_unlock });
		}
	}

	private bool IsPromoteBlocked()
	{
		int value = _currency.Value;
		int value2 = _promote.Level.Value;
		if (_rewardPromoteLevels.ContainsKey(value2))
		{
			List<int> list = _rewardPromoteLevels[value2];
			if (list[list.Count - 1] <= value)
			{
				return true;
			}
		}
		return false;
	}

	private bool CanSpend()
	{
		bool result = true;
		int value = _currency.Value;
		IReadOnlyList<IReadOnlyList<RewardWithManyConditions>> rewards = _relationship.Rewards;
		for (int i = 0; i < rewards.Count; i++)
		{
			RewardWithManyConditions rewardWithManyConditions = rewards[i][0];
			bool num = i == rewards.Count - 1;
			int requiredPointsForReward = _relationship.GetRequiredPointsForReward(i);
			if (num)
			{
				result = value < requiredPointsForReward;
				break;
			}
			if (value <= requiredPointsForReward)
			{
				result = rewardWithManyConditions.State.Value != EntityStatus.Blocked;
				break;
			}
		}
		return result;
	}

	private void SetCurrencies()
	{
		foreach (Present item in _presentsManager.Collection)
		{
			if (_walletProvider.TryGetContainer(item.CurrencyType, out var container))
			{
				item.SetContainer(container);
			}
		}
	}

	private void StartTrackPresent(PresentView presentView)
	{
		_presentCount = 0;
	}

	private void EndTrackPresent(PresentView presentView)
	{
		SentEvent(presentView);
		_presentCount = 0;
	}

	private void SentEvent(PresentView presentView)
	{
		if (0 < _presentCount)
		{
			_presentsAnalytic.SendSpendEvent(presentView.Source.Id, _presentCount, _relationshipId[0]);
		}
	}
}
