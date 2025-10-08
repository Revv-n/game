using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Characters;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.Presents;
using GreenT.HornyScapes.Presents.Services;
using GreenT.HornyScapes.Presents.UI;
using GreenT.HornyScapes.Relationships.Mappers;
using GreenT.HornyScapes.Relationships.Models;
using GreenT.HornyScapes.Relationships.Providers;
using GreenT.HornyScapes.Relationships.Services;
using GreenT.HornyScapes.Relationships.Views;
using GreenT.HornyScapes.Relationships.Views.Aminations;
using GreenT.Types;
using StripClub.Model;
using StripClub.Model.Cards;
using StripClub.UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Relationships.Windows;

public class RelationshipUiSetter : MonoView<(GreenT.HornyScapes.Characters.CharacterInfo Character, Relationship Relationship)>, IDisposable
{
	private const int Zero = 0;

	private const int One = 1;

	[SerializeField]
	private RectTransform _levelContainer;

	[SerializeField]
	private ScrollSnapController _scrollSnapController;

	[SerializeField]
	private PresentsWindowSetter _presentsWindowSetter;

	[SerializeField]
	private RelationshipBackgroundController _backgroundController;

	[SerializeField]
	private RelationshipsToolTip _toolTip;

	[SerializeField]
	private LovePointsClip _lovePointsAnimator;

	private RelationshipMapperProvider _relationshipMapperProvider;

	private RelationshipRewardMapperProvider _relationshipRewardMapperProvider;

	private RelationshipLevelContainer.ViewManager _relationshipLevelViewManager;

	private RewardViewProvider _rewardViewProvider;

	private LevelUpCommandHandler _levelUpCommandHandler;

	private ActiveRewardTracker _activeRewardTracker;

	private PresentsManager _presentsManager;

	private Currencies _currencies;

	private PresentsService _presentsService;

	private CardsCollection _cards;

	private BlockLevelTracker _blockLevelTracker;

	private RelationshipStatusTracker _statusTracker;

	private IDisposable _presentStream;

	private readonly List<RelationshipLevelContainer> _levelViews = new List<RelationshipLevelContainer>(32);

	private readonly List<BaseRewardView> _rewardViews = new List<BaseRewardView>(32);

	private readonly Subject<IReadOnlyList<BaseRewardView>> _rewardShowed = new Subject<IReadOnlyList<BaseRewardView>>();

	public IObservable<IReadOnlyList<BaseRewardView>> RewardsShowed => Observable.AsObservable<IReadOnlyList<BaseRewardView>>((IObservable<IReadOnlyList<BaseRewardView>>)_rewardShowed);

	[Inject]
	private void Init(RelationshipMapperProvider relationshipMapperProvider, RelationshipRewardMapperProvider relationshipRewardMapperProvider, RelationshipLevelContainer.ViewManager levelViewManager, RewardViewProvider rewardViewProvider, LevelUpCommandHandler levelUpCommandHandler, ActiveRewardTracker activeRewardTracker, PresentsManager presentsManager, Currencies currencies, PresentsService presentsService, CardsCollection cards, BlockLevelTracker blockLevelTracker, RelationshipStatusTracker statusTracker)
	{
		_relationshipMapperProvider = relationshipMapperProvider;
		_relationshipRewardMapperProvider = relationshipRewardMapperProvider;
		_relationshipLevelViewManager = levelViewManager;
		_rewardViewProvider = rewardViewProvider;
		_levelUpCommandHandler = levelUpCommandHandler;
		_activeRewardTracker = activeRewardTracker;
		_presentsManager = presentsManager;
		_currencies = currencies;
		_presentsService = presentsService;
		_cards = cards;
		_blockLevelTracker = blockLevelTracker;
		_statusTracker = statusTracker;
	}

	public override void Set((GreenT.HornyScapes.Characters.CharacterInfo Character, Relationship Relationship) source)
	{
		base.Set(source);
		Relationship item = base.Source.Relationship;
		int iD = item.ID;
		_relationshipLevelViewManager.HideAll();
		_rewardViewProvider.HideAll();
		_backgroundController.Clear(iD);
		_scrollSnapController.Clear();
		IPromote promoteOrDefault = _cards.GetPromoteOrDefault(base.Source.Character);
		_levelUpCommandHandler.Set(item);
		_statusTracker.Set(item);
		IReadOnlyList<RewardWithManyConditions>[] rewardsArray = item.Rewards.ToArray();
		CompositeIdentificator identificator = new CompositeIdentificator(iD);
		_presentsWindowSetter.Set(_presentsManager.Collection);
		List<int> rewardIds = GetRewardIds();
		List<int> rewardStatuses = GetRewardStatuses();
		List<int> rewardPromoteLevels = GetRewardPromoteLevels();
		int num = int.MaxValue;
		int num2 = int.MinValue;
		int num3 = rewardsArray.Length;
		int num4 = 1;
		ClearViews();
		RelationshipLevelContainer relationshipLevelContainer = null;
		for (int i = 0; i < num3; i++)
		{
			int num5 = rewardStatuses[i];
			bool flag = num5 < num;
			num = num5;
			int num6 = rewardPromoteLevels[i];
			bool num7 = num2 < num6;
			num2 = num6;
			if (flag)
			{
				relationshipLevelContainer = _relationshipLevelViewManager.GetView();
				relationshipLevelContainer.transform.SetAsLastSibling();
				_levelViews.Add(relationshipLevelContainer);
				num4++;
			}
			IReadOnlyList<RewardWithManyConditions> rewards = rewardsArray[i];
			BaseRewardView baseRewardView = _rewardViewProvider.Display(rewardIds[i], rewards);
			_rewardViews.Add(baseRewardView);
			_scrollSnapController.Add(baseRewardView);
			int startValue = ((i != 0) ? item.GetRequiredPointsForReward(i - 1) : 0);
			int requiredPointsForReward = item.GetRequiredPointsForReward(i);
			int statusStart = (flag ? (num4 - 1) : num4);
			int statusEnd = num4;
			bool num8 = i == 0;
			if (i + 1 != num3)
			{
				_ = rewardStatuses[i + 1] < num;
			}
			else
				_ = 0;
			IReadOnlyList<RewardWithManyConditions> rewardStart = (num8 ? null : rewardsArray[i - 1]);
			IReadOnlyList<RewardWithManyConditions> rewardEnd = rewardsArray[i];
			int countStart = ((i == 0) ? num5 : rewardStatuses[i - 1]);
			int countEnd = num5;
			baseRewardView.SetPromote(promoteOrDefault, num6, item);
			baseRewardView.SetStatus(statusStart, statusEnd, countStart, countEnd, rewardStart, rewardEnd);
			baseRewardView.SetTargetPoints(identificator, startValue, requiredPointsForReward);
			baseRewardView.SetActiveBlockedLevel(isActive: false);
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<RelationshipsToolTipOpener>(baseRewardView.TooltipRequested, (Action<RelationshipsToolTipOpener>)ShowTooltip), (Component)this);
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<BaseRewardView>(baseRewardView.ProgressChanged, (Action<BaseRewardView>)ScrollTo), (Component)this);
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<int>(baseRewardView.RewardClaimed, (Action<int>)item.ClaimReward), (Component)this);
			baseRewardView.transform.SetAsLastSibling();
			relationshipLevelContainer.Set(baseRewardView, num4, flag);
			_backgroundController.Set(iD, baseRewardView, num4, flag);
			if (num7)
			{
				_blockLevelTracker.AddBlockedReward(iD, baseRewardView);
			}
			_blockLevelTracker.AddReward(iD, baseRewardView);
		}
		_backgroundController.SetNeedAnimate(iD);
		List<BaseRewardView> visibleViews = _rewardViewProvider.VisibleViews;
		visibleViews.First((BaseRewardView x) => x.Source.Rewards == rewardsArray[0]).SetAsFirstInTrack();
		visibleViews.First(delegate(BaseRewardView x)
		{
			IReadOnlyList<RewardWithManyConditions> item2 = x.Source.Rewards;
			IReadOnlyList<RewardWithManyConditions>[] array = rewardsArray;
			return item2 == array[array.Length - 1];
		}).SetAsLastInTrack();
		_activeRewardTracker.Set(item);
		_presentStream?.Dispose();
		_presentStream = ObservableExtensions.Subscribe<PresentView>(_presentsService.Spended, (Action<PresentView>)OnPresentSpended);
		_presentStream = ObservableExtensions.Subscribe<(PresentView, PresentSpendFailReason)>(_presentsService.SpendFailed, (Action<(PresentView, PresentSpendFailReason)>)OnPresentSpendFailed);
		BaseRewardView currentRewardView = GetCurrentRewardView();
		if (currentRewardView != null)
		{
			RectTransform handleRectTransform = currentRewardView.GetHandleRectTransform();
			_toolTip.transform.SetParent(handleRectTransform, worldPositionStays: false);
		}
	}

	public void Dispose()
	{
		_presentStream?.Dispose();
	}

	private void OnEnable()
	{
		PrepareActivate();
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Unit>(Observable.ObserveOnMainThread<Unit>(Observable.NextFrame((FrameCountType)0)), (Action<Unit>)delegate
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(_levelContainer);
			Activate();
		}), (Component)this);
	}

	private void OnDestroy()
	{
		_blockLevelTracker.Clear();
	}

	private void PrepareActivate()
	{
		var (characterInfo, relationship) = base.Source;
		if (characterInfo != null || relationship != null)
		{
			int iD = base.Source.Relationship.ID;
			_blockLevelTracker.TryPrepareAnimation(iD);
		}
	}

	private void Activate()
	{
		var (characterInfo, relationship) = base.Source;
		if (characterInfo != null || relationship != null)
		{
			int iD = base.Source.Relationship.ID;
			_backgroundController.TryStartAnimation(iD);
			_blockLevelTracker.TryStartAnimation(iD);
			BaseRewardView baseRewardView = GetCurrentRewardView();
			if (baseRewardView == null)
			{
				List<BaseRewardView> rewardViews = _rewardViews;
				baseRewardView = rewardViews[rewardViews.Count - 1];
			}
			ScrollTo(baseRewardView);
			_rewardShowed?.OnNext((IReadOnlyList<BaseRewardView>)_rewardViewProvider.VisibleViews);
		}
	}

	private List<int> GetRewardIds()
	{
		return GetRewardData((RelationshipRewardMapper m) => m.id);
	}

	private List<int> GetRewardStatuses()
	{
		return GetRewardData((RelationshipRewardMapper m) => m.status_number);
	}

	private List<int> GetRewardPromoteLevels()
	{
		return GetRewardData((RelationshipRewardMapper m) => m.promote_to_unlock);
	}

	private List<T> GetRewardData<T>(Func<RelationshipRewardMapper, T> selector)
	{
		int[] rewards = _relationshipMapperProvider.Get(base.Source.Relationship.ID).rewards;
		List<T> list = new List<T>(rewards.Length);
		int[] array = rewards;
		foreach (int id in array)
		{
			RelationshipRewardMapper arg = _relationshipRewardMapperProvider.Get(id);
			list.Add(selector(arg));
		}
		return list;
	}

	private void ShowTooltip(RelationshipsToolTipOpener toolTipOpener)
	{
		Relationship item = base.Source.Relationship;
		CompositeIdentificator identificator = new CompositeIdentificator(item.ID);
		int value = _currencies.Get(CurrencyType.LovePoints, identificator).Value;
		BaseRewardView baseRewardView = GetCurrentRewardView();
		if (baseRewardView == null)
		{
			List<BaseRewardView> rewardViews = _rewardViews;
			baseRewardView = rewardViews[rewardViews.Count - 1];
		}
		RectTransform handleRectTransform = baseRewardView.GetHandleRectTransform();
		ScrollTo(baseRewardView);
		toolTipOpener.ShowToolTip(_toolTip, handleRectTransform, value);
	}

	private BaseRewardView GetCurrentRewardView()
	{
		var (characterInfo, relationship) = base.Source;
		if ((characterInfo == null && relationship == null) || base.Source.Relationship == null)
		{
			return null;
		}
		Relationship item = base.Source.Relationship;
		CompositeIdentificator identificator = new CompositeIdentificator(item.ID);
		int value = _currencies.Get(CurrencyType.LovePoints, identificator).Value;
		foreach (BaseRewardView rewardView in _rewardViews)
		{
			int startLovePoints = rewardView.GetStartLovePoints();
			int targetLovePoints = rewardView.GetTargetLovePoints();
			if (value >= startLovePoints && targetLovePoints >= value)
			{
				return rewardView;
			}
		}
		return null;
	}

	private void ScrollTo(BaseRewardView snapTarget)
	{
		int target = ((snapTarget != null) ? _rewardViewProvider.VisibleViews.IndexOf(snapTarget) : 0);
		_scrollSnapController.SnapTo(target);
	}

	private void ClearViews()
	{
		foreach (RelationshipLevelContainer levelView in _levelViews)
		{
			levelView.Display(display: false);
		}
		_levelViews.Clear();
		foreach (BaseRewardView rewardView in _rewardViews)
		{
			rewardView.Display(display: false);
		}
		_rewardViews.Clear();
	}

	private void OnPresentSpended(PresentView presentView)
	{
		BaseRewardView currentRewardView = GetCurrentRewardView();
		if (!(currentRewardView == null))
		{
			Transform startPoint = presentView.transform;
			RectTransform dropTarget = currentRewardView.GetDropTarget();
			_lovePointsAnimator.Init(startPoint, dropTarget);
			_lovePointsAnimator.Play();
		}
	}

	private void OnPresentSpendFailed((PresentView PresentView, PresentSpendFailReason Reason) info)
	{
		var (presentView, _) = info;
		switch (info.Reason)
		{
		case PresentSpendFailReason.NotEnought:
			presentView.RequestTooltip();
			break;
		case PresentSpendFailReason.PromoteBlocked:
		{
			foreach (BaseRewardView rewardView in _rewardViews)
			{
				rewardView.AnimateBlockedLevel();
			}
			break;
		}
		}
	}
}
