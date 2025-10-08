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
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<(CalendarModel, BattlePass, RewardWithManyConditions)>(Observable.Concat<(CalendarModel, BattlePass, RewardWithManyConditions)>(Observable.Select<(CalendarModel, BattlePass, RewardWithManyConditions), IObservable<(CalendarModel, BattlePass, RewardWithManyConditions)>>(Observable.Do<(CalendarModel, BattlePass, RewardWithManyConditions)>(Observable.Merge<(CalendarModel, BattlePass, RewardWithManyConditions)>(Observable.Select<CalendarModel, IObservable<(CalendarModel, BattlePass, RewardWithManyConditions)>>(Observable.Do<CalendarModel>(_calendarQueue.OnCalendarActiveNotNull(EventStructureType.BattlePass), (Action<CalendarModel>)Init), (Func<CalendarModel, IObservable<(CalendarModel, BattlePass, RewardWithManyConditions)>>)OnPremiumRewardUpdate)), (Action<(CalendarModel, BattlePass, RewardWithManyConditions)>)FillList), (Func<(CalendarModel, BattlePass, RewardWithManyConditions), IObservable<(CalendarModel, BattlePass, RewardWithManyConditions)>>)(((CalendarModel calendar, BattlePass battlePass, RewardWithManyConditions reward) value) => Observable.Delay<(CalendarModel, BattlePass, RewardWithManyConditions)>(Observable.Return<(CalendarModel, BattlePass, RewardWithManyConditions)>(value), TimeSpan.FromSeconds(0.10000000149011612))))), (Action<(CalendarModel, BattlePass, RewardWithManyConditions)>)SendEventByPass), (ICollection<IDisposable>)onNewStream);
	}

	private void Init(CalendarModel calendarModel)
	{
	}

	public override void SendEventByPass((CalendarModel calendar, BattlePass battlePass, RewardWithManyConditions reward) tuple)
	{
		if (_rewardCounts.Count != 0)
		{
			(int, int, int) tuple2 = _rewardCounts.Dequeue();
			BattlePassPremiumRewardAmplitudeEvent battlePassPremiumRewardAmplitudeEvent = new BattlePassPremiumRewardAmplitudeEvent(tuple2.Item1, tuple2.Item2, tuple2.Item3);
			((IAnalyticSender<AmplitudeEvent>)(object)amplitude).AddEvent((AmplitudeEvent)battlePassPremiumRewardAmplitudeEvent);
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
		return Observable.Select<RewardWithManyConditions, (CalendarModel, BattlePass, RewardWithManyConditions)>(Observable.Where<RewardWithManyConditions>(Observable.TakeWhile<RewardWithManyConditions>(battlePass.PremiumRewardContainer.OnRewardUpdate(), (Func<RewardWithManyConditions, bool>)((RewardWithManyConditions _) => _calendarQueue.IsCalendarActive(calendarModel))), (Func<RewardWithManyConditions, bool>)((RewardWithManyConditions reward) => reward.State.Value == EntityStatus.Complete)), (Func<RewardWithManyConditions, (CalendarModel, BattlePass, RewardWithManyConditions)>)((RewardWithManyConditions reward) => (calendarModel: calendarModel, battlePass: battlePass, reward: reward)));
	}
}
