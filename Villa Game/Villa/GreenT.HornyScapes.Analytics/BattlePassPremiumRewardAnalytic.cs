using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Events;
using Merge.Meta.RoomObjects;
using UniRx;

namespace GreenT.HornyScapes.Analytics;

public class BattlePassPremiumRewardAnalytic : BaseEntityAnalytic<(CalendarModel calendar, BattlePass battlePass, RewardWithManyConditions reward)>
{
	private readonly BattlePassSettingsProvider _battlePassSettingsProvider;

	private readonly CalendarQueue _calendarQueue;

	private Queue<(int, int, int)> _rewardCounts;

	public BattlePassPremiumRewardAnalytic(IAmplitudeSender<AmplitudeEvent> amplitude, CalendarQueue calendarQueue, BattlePassSettingsProvider battlePassSettingsProvider)
		: base(amplitude)
	{
		_calendarQueue = calendarQueue;
		_battlePassSettingsProvider = battlePassSettingsProvider;
		_rewardCounts = new Queue<(int, int, int)>();
	}

	public override void Track()
	{
		ClearStreams();
		TrackGotRewards();
	}

	private void TrackGotRewards()
	{
		(from value in _calendarQueue.OnCalendarActiveNotNull(EventStructureType.BattlePass).Do(Init).Select(OnPremiumRewardUpdate)
				.Merge()
				.Do(FillList)
			select Observable.Return(value).Delay(TimeSpan.FromSeconds(0.10000000149011612))).Concat().Subscribe(SendEventByPass).AddTo(onNewStream);
	}

	private void Init(CalendarModel calendarModel)
	{
	}

	public override void SendEventByPass((CalendarModel calendar, BattlePass battlePass, RewardWithManyConditions reward) tuple)
	{
		if (_rewardCounts.Count != 0)
		{
			(int, int, int) tuple2 = _rewardCounts.Dequeue();
			BattlePassPremiumRewardAmplitudeEvent analyticsEvent = new BattlePassPremiumRewardAmplitudeEvent(tuple2.Item1, tuple2.Item2, tuple2.Item3);
			amplitude.AddEvent(analyticsEvent);
		}
	}

	private void FillList((CalendarModel calendar, BattlePass battlePass, RewardWithManyConditions reward) tuple)
	{
		int uniqID = tuple.calendar.UniqID;
		int iD = tuple.battlePass.ID;
		int item = tuple.battlePass.PremiumRewardContainer.Rewards.Count((RewardWithManyConditions x) => x.IsRewarded || x.IsComplete);
		_rewardCounts.Enqueue((uniqID, iD, item));
	}

	private IObservable<(CalendarModel calendar, BattlePass battlePass, RewardWithManyConditions reward)> OnPremiumRewardUpdate(CalendarModel calendarModel)
	{
		BattlePass battlePass = _battlePassSettingsProvider.GetBattlePass(calendarModel.BalanceId);
		return from reward in battlePass.PremiumRewardContainer.OnRewardUpdate().TakeWhile((RewardWithManyConditions _) => _calendarQueue.IsCalendarActive(calendarModel))
			where reward.State.Value == EntityStatus.Complete
			select (calendarModel: calendarModel, battlePass: battlePass, reward: reward);
	}
}
