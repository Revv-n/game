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
		_providerSubscribe = _calendarQueue.OnCalendarActiveNotNull(EventStructureType.BattlePass).Subscribe(BattlePassChanged);
	}

	private void BattlePassChanged(CalendarModel calendarModel)
	{
		Reset();
		if (_battlePassSettingsProvider.TryGetBattlePass(calendarModel.BalanceId, out battlePass))
		{
			currentCalendarModel = calendarModel;
			_calendarStateStream = currentCalendarModel.CalendarState.Where((EntityStatus p) => p == EntityStatus.Complete).Subscribe(delegate
			{
				OnCollectedReward();
			});
			_rewardSubscribe = (from p in battlePass.AllRewardContainer.OnRewardUpdate()
				where p.State.Value == EntityStatus.Rewarded
				select p).Subscribe(delegate
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
