using System;
using GreenT.HornyScapes.Events;
using GreenT.UI;
using JetBrains.Annotations;
using Merge.Meta.RoomObjects;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.BattlePassSpace;

[UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
public class BattlePassCheckingForCompletion : IInitializable, IDisposable
{
	private readonly BattlePassSettingsProvider _battlePassSettingsProvider;

	private readonly CalendarQueue _calendarQueue;

	private readonly WindowGroupID _battlePassWindowGroup;

	private readonly IWindowsManager _windowsManager;

	private CalendarModel currentCalendarModel;

	private BattlePass battlePass;

	private IDisposable _providerSubscribe;

	private IDisposable _rewardSubscribe;

	private IDisposable _calendarStateStream;

	private BattlePassCheckingForCompletion(CalendarQueue calendarQueue, WindowGroupID battlePassWindowGroup, IWindowsManager windowsManager, BattlePassSettingsProvider battlePassSettingsProvider)
	{
		_battlePassSettingsProvider = battlePassSettingsProvider;
		_calendarQueue = calendarQueue;
		_battlePassWindowGroup = battlePassWindowGroup;
		_windowsManager = windowsManager;
	}

	public void Initialize()
	{
		_providerSubscribe = ObservableExtensions.Subscribe<CalendarModel>(_calendarQueue.OnCalendarActiveNotNull(EventStructureType.BattlePass), (Action<CalendarModel>)BattlePassChanged);
	}

	private void BattlePassChanged(CalendarModel calendarModel)
	{
		Reset();
		if (_battlePassSettingsProvider.TryGetBattlePass(calendarModel.BalanceId, out battlePass))
		{
			currentCalendarModel = calendarModel;
			_calendarStateStream = ObservableExtensions.Subscribe<EntityStatus>(Observable.Where<EntityStatus>((IObservable<EntityStatus>)currentCalendarModel.CalendarState, (Func<EntityStatus, bool>)((EntityStatus p) => p == EntityStatus.Complete)), (Action<EntityStatus>)delegate
			{
				OnCollectedReward();
			});
			_rewardSubscribe = ObservableExtensions.Subscribe<RewardWithManyConditions>(Observable.Where<RewardWithManyConditions>(battlePass.AllRewardContainer.OnRewardUpdate(), (Func<RewardWithManyConditions, bool>)((RewardWithManyConditions p) => p.State.Value == EntityStatus.Rewarded)), (Action<RewardWithManyConditions>)delegate
			{
				OnCollectedReward();
			});
		}
	}

	private void OnCollectedReward()
	{
		EntityStatus value = currentCalendarModel.CalendarState.Value;
		if (!battlePass.HasRewards() && (value == EntityStatus.Complete || value == EntityStatus.Rewarded))
		{
			currentCalendarModel.SetRewarded();
		}
	}

	private void Reset()
	{
		_calendarStateStream?.Dispose();
		_rewardSubscribe?.Dispose();
		currentCalendarModel = null;
		battlePass = null;
	}

	public void Dispose()
	{
		Reset();
		_providerSubscribe?.Dispose();
	}
}
