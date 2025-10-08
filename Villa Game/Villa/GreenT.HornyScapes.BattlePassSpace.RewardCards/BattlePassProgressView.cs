using System;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.Events.BattlePassRewardCards;
using Merge.Meta.RoomObjects;
using StripClub.Extensions;
using StripClub.UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.BattlePassSpace.RewardCards;

public class BattlePassProgressView : MonoView<CalendarModel>, IInitializable, IDisposable
{
	private const int FreeState = 0;

	private const int PremiumPurchasedState = 1;

	private const int BattlePassCompleteState = 2;

	[SerializeField]
	private LocalizedTextMeshPro _titleField;

	[SerializeField]
	private LocalizedTextMeshPro _titleShadow;

	[SerializeField]
	private MonoTimer _offerTimer;

	[SerializeField]
	private SliderUpBattlePassProgress _sliderUpBattlePassProgress;

	[SerializeField]
	private ScrollContentScroller _contentScroller;

	[SerializeField]
	private StatableComponentGroup _holderGroup;

	[SerializeField]
	private Image _arrow;

	[SerializeField]
	private Image _background;

	[SerializeField]
	private Image _headerImage;

	[SerializeField]
	private Image _girlImage;

	[SerializeField]
	private Image _activationBackground;

	[SerializeField]
	private Image _activatedBackground;

	[SerializeField]
	private Transform _rewardLevelViewContrainer;

	[SerializeField]
	private Button _closeButton;

	[SerializeField]
	private Button _claimAllButton;

	private TimeHelper _timeHelper;

	private CalendarQueue _calendarQueue;

	private BattlePassProvider _battlePassProvider;

	private BattlePassSettingsProvider _battlePassSettingsProvider;

	private BattlePassRewardLevelManager _battlePassRewardLevelManager;

	private IDisposable _rewardUpdateStream;

	private IDisposable _claimButtonSubscribe;

	private BattlePass _battlePass;

	private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

	[Inject]
	public void Construct(BattlePassProvider provider, TimeHelper timeHelper, BattlePassSettingsProvider battlePassSettingsProvider, CalendarQueue calendarFacade, BattlePassRewardLevelManager battlePassRewardLevelManager)
	{
		_timeHelper = timeHelper;
		_battlePassProvider = provider;
		_calendarQueue = calendarFacade;
		_battlePassSettingsProvider = battlePassSettingsProvider;
		_battlePassRewardLevelManager = battlePassRewardLevelManager;
	}

	public void Initialize()
	{
		_compositeDisposable.Clear();
		(from calendar in _calendarQueue.OnCalendarActiveNotNull(EventStructureType.BattlePass)
			select (calendar: calendar, _battlePassSettingsProvider.GetBattlePass(calendar.BalanceId)) into tuple
			where tuple.Item2 != null
			select tuple).Do(delegate((CalendarModel calendar, BattlePass) tuple)
		{
			TryRestoreState(tuple.Item2);
		}).Subscribe(ApplyData).AddTo(_compositeDisposable);
		_battlePassProvider.CalendarChangeProperty.Where(((CalendarModel calendar, BattlePass battlePass) tuple) => tuple.battlePass != null).SelectMany(((CalendarModel calendar, BattlePass battlePass) tuple) => tuple.battlePass.Data.StartData.PremiumPurchasedProperty).Subscribe(delegate(bool status)
		{
			int stateNumber = (status ? 1 : 0);
			_holderGroup.Set(stateNumber);
		})
			.AddTo(_compositeDisposable);
		(from status in (from tuple in _battlePassProvider.CalendarChangeProperty
				where tuple.battlePass != null
				select tuple.calendar).SelectMany((CalendarModel tuple) => tuple.CalendarState)
			where status == EntityStatus.Complete
			select status).Subscribe(delegate
		{
			_holderGroup.Set(2);
		}).AddTo(_compositeDisposable);
		_closeButton.OnClickAsObservable().Subscribe(delegate
		{
			AtClose();
		}).AddTo(_compositeDisposable);
	}

	public void Dispose()
	{
		_compositeDisposable.Dispose();
		_rewardUpdateStream?.Dispose();
		_claimButtonSubscribe?.Dispose();
	}

	private void TryRestoreState(BattlePass battlePass)
	{
		_holderGroup.Set(battlePass.Data.StartData.PremiumPurchasedProperty.Value ? 1 : 0);
	}

	private void OnEnable()
	{
		if (_battlePass != null)
		{
			if (!_battlePass.Data.StartData.FirstStartedProgress.Value)
			{
				_battlePass.Data.StartData.SetIsNotFirstStartedProgress();
			}
			ShowRewardCards(_battlePass);
			CheckCompleteRewards();
		}
	}

	private void ApplyData((CalendarModel calendarModel, BattlePass battlePass) tuple)
	{
		Set(tuple.calendarModel);
		BattlePassBundleData bundle = tuple.battlePass.Bundle;
		_sliderUpBattlePassProgress.Reset();
		_sliderUpBattlePassProgress.Set(tuple.battlePass);
		_battlePass = tuple.battlePass;
		_titleField.Init(bundle.TitleKeyLoc);
		_titleShadow.Init(bundle.TitleKeyLoc);
		_offerTimer.Init(tuple.calendarModel.Duration, _timeHelper.UseCombineFormat);
		_activationBackground.sprite = bundle.ActivationBackground;
		_activatedBackground.sprite = bundle.ActivatedBackground;
		_background.sprite = bundle.AnnouncementBackground;
		_girlImage.sprite = bundle.ProgressGirl;
		_headerImage.sprite = bundle.AnnouncementTitleBackground;
		_arrow.sprite = bundle.PremiumTrackArrow;
		_rewardUpdateStream = _battlePass.OnRewardUpdate.Subscribe(delegate
		{
			CheckCompleteRewards();
		});
		_claimButtonSubscribe = _claimAllButton.OnClickAsObservable().Subscribe(delegate
		{
			_battlePass.TryCollectAllRewards();
		});
	}

	private void ShowRewardCards(BattlePass battlePass)
	{
		_battlePassRewardLevelManager.HideAll();
		int targetIndex = -1;
		for (int i = 0; i < battlePass.Data.RewardPairQueue.Cases.Count; i++)
		{
			BattlePassRewardPairData battlePassRewardPairData = battlePass.Data.RewardPairQueue.Cases[i];
			BattlePassRewardLevelView battlePassRewardLevelView = _battlePassRewardLevelManager.Display(battlePassRewardPairData);
			battlePassRewardLevelView.UpdateRewardHolder(battlePass);
			battlePassRewardLevelView.transform.SetParent(_rewardLevelViewContrainer);
			battlePassRewardLevelView.transform.SetSiblingIndex(i);
			if (targetIndex == -1 && battlePassRewardPairData.HasAnyRewardComplete())
			{
				targetIndex = i;
			}
		}
		Observable.NextFrame().ObserveOnMainThread().Subscribe(delegate
		{
			_contentScroller.ScrollToIndex(targetIndex);
		})
			.AddTo(this);
	}

	private void CheckCompleteRewards()
	{
		_battlePass.HasUncollectedRewards();
		_claimAllButton.gameObject.SetActive(value: false);
	}

	private void AtClose()
	{
		_battlePassRewardLevelManager.HideAll();
	}
}
