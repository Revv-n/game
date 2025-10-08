using System;
using System.Linq;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Events;
using Merge.Meta.RoomObjects;
using UniRx;

namespace GreenT.HornyScapes.Analytics;

public class EventRewardAnalytic : BaseEntityAnalytic<(CalendarModel calendar, NotifyReward reward)>
{
	private readonly CalendarQueue _calendarQueue;

	private readonly EventSettingsProvider _eventSettingsProvider;

	private readonly BattlePassSettingsProvider _battlePassSettingsProvider;

	public EventRewardAnalytic(IAmplitudeSender<AmplitudeEvent> amplitude, CalendarQueue calendarQueue, EventSettingsProvider eventSettingsProvider, BattlePassSettingsProvider battlePassSettingsProvider)
		: base(amplitude)
	{
		_calendarQueue = calendarQueue;
		_eventSettingsProvider = eventSettingsProvider;
		_battlePassSettingsProvider = battlePassSettingsProvider;
	}

	public override void Track()
	{
		onNewStream.Clear();
		TrackGotRewards();
	}

	private void TrackGotRewards()
	{
		(from _reward in _calendarQueue.OnCalendarActiveNotNull(EventStructureType.Event).Do(Init).SelectMany((Func<CalendarModel, IObservable<NotifyReward>>)OnAnyRewardUpdate)
			select (_calendarQueue.GetActiveCalendar(EventStructureType.Event), _reward: _reward)).Subscribe(SendEventByPass).AddTo(onNewStream);
		(from calendar in _calendarQueue.OnCalendarActiveNotNull(EventStructureType.Event).Do(Init)
			where calendar.EventType == EventStructureType.Event && IsEventBattlePass(calendar)
			select calendar).Subscribe(CheckEventBattlePass).AddTo(onNewStream);
	}

	private void Init(CalendarModel calendarModel)
	{
	}

	public override void SendEventByPass((CalendarModel calendar, NotifyReward reward) tuple)
	{
		if (_eventSettingsProvider.TryGetEvent(tuple.calendar.BalanceId, out var @event))
		{
			int uniqID = tuple.calendar.UniqID;
			int eventId = @event.EventId;
			int filteredRewardsCount = tuple.calendar.RewardHolder.GetFilteredRewardsCount(new EntityStatus[1] { EntityStatus.Rewarded });
			EventRewardAmplitudeEvent analyticsEvent = new EventRewardAmplitudeEvent(uniqID, eventId, filteredRewardsCount, 0, 0);
			amplitude.AddEvent(analyticsEvent);
		}
	}

	private IObservable<NotifyReward> OnAnyRewardUpdate(CalendarModel calendarModel)
	{
		return from _reward in _eventSettingsProvider.GetEvent(calendarModel.BalanceId).OnRewardUpdate().TakeWhile((NotifyReward _) => _calendarQueue.IsCalendarActive(calendarModel))
			where _reward.State.Value == EntityStatus.Rewarded
			select _reward;
	}

	private bool IsEventBattlePass(CalendarModel calendar)
	{
		return GetBattlePassId(calendar) != 0;
	}

	private int GetBattlePassId(CalendarModel calendar)
	{
		return (calendar.EventMapper as EventMapper).bp_id;
	}

	private void CheckEventBattlePass(CalendarModel calendar)
	{
		int calendarId = calendar.UniqID;
		int battlePassId = GetBattlePassId(calendar);
		Event @event = _eventSettingsProvider.GetEvent(calendar.BalanceId);
		int eventId = @event.EventId;
		if (_battlePassSettingsProvider.TryGetBattlePass(battlePassId, out var battlePass))
		{
			(from reward in battlePass.FreeRewardContainer.OnRewardUpdate()
				select reward.IsRewarded).Subscribe(delegate
			{
				SendEventBattlePassRewardCollected(calendarId, eventId, battlePass);
			}).AddTo(onNewStream);
			(from reward in battlePass.PremiumRewardContainer.OnRewardUpdate()
				select reward.IsRewarded).Subscribe(delegate
			{
				SendEventBattlePassRewardCollected(calendarId, eventId, battlePass);
			}).AddTo(onNewStream);
		}
	}

	private void SendEventBattlePassRewardCollected(int calendarId, int eventId, BattlePass battlePass)
	{
		int free_track = battlePass.FreeRewardContainer.Rewards.Count((RewardWithManyConditions reward) => reward.IsRewarded);
		int paid_track = battlePass.PremiumRewardContainer.Rewards.Count((RewardWithManyConditions reward) => reward.IsRewarded);
		EventRewardAmplitudeEvent analyticsEvent = new EventRewardAmplitudeEvent(calendarId, eventId, 0, free_track, paid_track);
		amplitude.AddEvent(analyticsEvent);
	}
}
