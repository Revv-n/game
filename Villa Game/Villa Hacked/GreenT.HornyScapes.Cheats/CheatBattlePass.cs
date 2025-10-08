using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.MergeCore;
using Merge;
using Merge.Meta.RoomObjects;
using StripClub.Model;
using StripClub.Model.Shop;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Cheats;

public class CheatBattlePass : MonoBehaviour, IDisposable
{
	[SerializeField]
	private Toggle _premiumToggle;

	public Button ChanceBattlePass;

	public Button AllChanceChange;

	public GameObject AllChance;

	public TMP_Text AllChanceText;

	private int _allChanceValue = 100;

	public GameObject RealChance;

	public Button BattlePassPoints;

	public Image BattlePassPointsIcon;

	public Button ClaimAllRewardsButton;

	public Button ClaimAllRewardButtonSilent;

	public Button ClaimPremiumLootboxButton;

	public Button ClaimPremiumExpensiveLootboxButton;

	public TMP_InputField InputField;

	public Gradient Gradient;

	private ICurrencyProcessor _currencyProcessor;

	private BattlePass _currentBattlePass;

	private IDisposable _allChanceChangeStream;

	private IDisposable _chanceBattlePassStream;

	private IDisposable _battlePassPointsStream;

	private IDisposable _currentCalendarStream;

	private IDisposable _battlePassPremiumStream;

	private CompositeDisposable _claimAllRewardsStream;

	private MergePointsController _mergePointsController;

	private CompositeDisposable _claimBattlePassStream;

	[Inject]
	private void InnerInit(BattlePassProvider provider, ICurrencyProcessor currencyProcessor, BattlePassSettingsProvider battlePassSettingsProvider, MergePointsController mergePointsController)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected O, but got Unknown
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Expected O, but got Unknown
		_mergePointsController = mergePointsController;
		_currencyProcessor = currencyProcessor;
		Dispose();
		_claimAllRewardsStream = new CompositeDisposable();
		_claimBattlePassStream = new CompositeDisposable();
		_currentCalendarStream = ObservableExtensions.Subscribe<BattlePass>(Observable.Select<(CalendarModel, BattlePass), BattlePass>(Observable.Where<(CalendarModel, BattlePass)>((IObservable<(CalendarModel, BattlePass)>)provider.CalendarChangeProperty, (Func<(CalendarModel, BattlePass), bool>)(((CalendarModel calendar, BattlePass battlePass) tuple) => tuple.calendar != null)), (Func<(CalendarModel, BattlePass), BattlePass>)(((CalendarModel calendar, BattlePass battlePass) tuple) => battlePassSettingsProvider.GetBattlePass(tuple.calendar.BalanceId))), (Action<BattlePass>)OnBattlePassChanged);
		_chanceBattlePassStream = ObservableExtensions.Subscribe<Unit>(UnityUIComponentExtensions.OnClickAsObservable(ChanceBattlePass), (Action<Unit>)delegate
		{
			OnChangeChance();
		});
		_allChanceChangeStream = ObservableExtensions.Subscribe<Unit>(UnityUIComponentExtensions.OnClickAsObservable(AllChanceChange), (Action<Unit>)delegate
		{
			OnAllChanceChange();
		});
		_battlePassPointsStream = ObservableExtensions.Subscribe<Unit>(UnityUIComponentExtensions.OnClickAsObservable(BattlePassPoints), (Action<Unit>)delegate
		{
			OnBattlePassPoints();
		});
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Unit>(UnityUIComponentExtensions.OnClickAsObservable(ClaimAllRewardsButton), (Action<Unit>)delegate
		{
			OnClaimAllRewards();
		}), (ICollection<IDisposable>)_claimAllRewardsStream);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Unit>(UnityUIComponentExtensions.OnClickAsObservable(ClaimAllRewardButtonSilent), (Action<Unit>)delegate
		{
			OnClaimAllRewardsSilent();
		}), (ICollection<IDisposable>)_claimAllRewardsStream);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Unit>(UnityUIComponentExtensions.OnClickAsObservable(ClaimPremiumLootboxButton), (Action<Unit>)delegate
		{
			OnClaimPremiumLootbox();
		}), (ICollection<IDisposable>)_claimBattlePassStream);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Unit>(UnityUIComponentExtensions.OnClickAsObservable(ClaimPremiumExpensiveLootboxButton), (Action<Unit>)delegate
		{
			OnClaimPremiumLootbox(expensive: true);
		}), (ICollection<IDisposable>)_claimBattlePassStream);
		_battlePassPremiumStream = ObservableExtensions.Subscribe<bool>(Observable.Skip<bool>(UnityUIComponentExtensions.OnValueChangedAsObservable(_premiumToggle), 1), (Action<bool>)delegate(bool value)
		{
			provider.CalendarChangeProperty.Value.Item2?.Data.StartData.SetPremiumPurchased(value);
		});
	}

	private void OnBattlePassPoints()
	{
		_currencyProcessor.TryAdd(CurrencyType.BP, ReadValue());
	}

	private IEnumerable<RewardWithManyConditions> GetUncollectedBattlepassRewards()
	{
		return _currentBattlePass.AllRewardContainer.Rewards.Where((RewardWithManyConditions reward) => reward.IsComplete);
	}

	private void OnClaimAllRewards()
	{
		if (_currentBattlePass == null)
		{
			return;
		}
		foreach (RewardWithManyConditions uncollectedBattlepassReward in GetUncollectedBattlepassRewards())
		{
			uncollectedBattlepassReward.TryCollectReward();
		}
	}

	private void OnClaimPremiumLootbox(bool expensive = false)
	{
		if (_currentBattlePass != null)
		{
			_currentBattlePass.Data.StartData.SetPurchaseComplete(value: true);
			_currentBattlePass.Data.StartData.SetPremiumPurchased(value: true);
			((!expensive) ? ((Lot)_currentBattlePass.PremiumLots.FirstOrDefault()) : ((Lot)_currentBattlePass.PremiumLots.LastOrDefault()))?.Content.AddCurrentToPlayer();
		}
	}

	private void OnClaimAllRewardsSilent()
	{
		if (_currentBattlePass == null)
		{
			return;
		}
		foreach (RewardWithManyConditions uncollectedBattlepassReward in GetUncollectedBattlepassRewards())
		{
			uncollectedBattlepassReward.Content.AddToPlayer();
			uncollectedBattlepassReward.ForceSetState(EntityStatus.Rewarded);
		}
	}

	private void OnAllChanceChange()
	{
		_allChanceValue = ReadValue();
		AllChanceText.text = $"{_allChanceValue}%";
		AllChanceText.color = Gradient.Evaluate((float)_allChanceValue / 100f);
		_mergePointsController.IsDroppedLogics.SetChance(_allChanceValue);
	}

	private void OnChangeChance()
	{
		AllChance.SetActive(!AllChance.activeSelf);
		AllChanceChange.gameObject.SetActive(AllChance.activeSelf);
		RealChance.SetActive(!RealChance.activeSelf);
	}

	private void OnBattlePassChanged(BattlePass battlePass)
	{
		_currentBattlePass = battlePass;
		if (battlePass == null)
		{
			ChanceBattlePass.gameObject.SetActive(value: false);
			BattlePassPoints.gameObject.SetActive(value: false);
			ClaimAllRewardsButton.gameObject.SetActive(value: false);
			ClaimAllRewardButtonSilent.gameObject.SetActive(value: false);
		}
		else
		{
			Reset();
			ChanceBattlePass.gameObject.SetActive(value: true);
			BattlePassPoints.gameObject.SetActive(value: true);
			ClaimAllRewardsButton.gameObject.SetActive(value: true);
			ClaimAllRewardButtonSilent.gameObject.SetActive(value: true);
			BattlePassPointsIcon.sprite = battlePass.CurrentViewSettings.Currency;
		}
	}

	private int ReadValue()
	{
		int result;
		int result2 = ((!int.TryParse(InputField.text, out result)) ? 1 : result);
		InputField.text = string.Empty;
		return result2;
	}

	private void OnDestroy()
	{
		Dispose();
	}

	private void Reset()
	{
		_allChanceValue = 100;
		AllChanceText.text = $"{_allChanceValue}%";
		AllChanceText.color = Gradient.Evaluate((float)_allChanceValue / 100f);
		AllChance.SetActive(value: false);
		AllChanceChange.SetActive(active: false);
		RealChance.SetActive(value: true);
	}

	public void Dispose()
	{
		_allChanceChangeStream?.Dispose();
		_chanceBattlePassStream?.Dispose();
		_battlePassPointsStream?.Dispose();
		_currentCalendarStream?.Dispose();
		CompositeDisposable claimAllRewardsStream = _claimAllRewardsStream;
		if (claimAllRewardsStream != null)
		{
			claimAllRewardsStream.Dispose();
		}
		_battlePassPremiumStream?.Dispose();
		CompositeDisposable claimBattlePassStream = _claimBattlePassStream;
		if (claimBattlePassStream != null)
		{
			claimBattlePassStream.Dispose();
		}
	}
}
