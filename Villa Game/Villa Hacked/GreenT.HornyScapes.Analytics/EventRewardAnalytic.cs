using System;
using System.Collections.Generic;
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
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<(CalendarModel, NotifyReward)>(Observable.Select<NotifyReward, (CalendarModel, NotifyReward)>(Observable.SelectMany<CalendarModel, NotifyReward>(Observable.Do<CalendarModel>(_calendarQueue.OnCalendarActiveNotNull(EventStructureType.Event), (Action<CalendarModel>)Init), (Func<CalendarModel, IObservable<NotifyReward>>)OnAnyRewardUpdate), (Func<NotifyReward, (CalendarModel, NotifyReward)>)((NotifyReward _reward) => (_calendarQueue.GetActiveCalendar(EventStructureType.Event), _reward: _reward))), (Action<(CalendarModel, NotifyReward)>)SendEventByPass), (ICollection<IDisposable>)onNewStream);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<CalendarModel>(Observable.Where<CalendarModel>(Observable.Do<CalendarModel>(_calendarQueue.OnCalendarActiveNotNull(EventStructureType.Event), (Action<CalendarModel>)Init), (Func<CalendarModel, bool>)((CalendarModel calendar) => calendar.EventType == EventStructureType.Event && IsEventBattlePass(calendar))), (Action<CalendarModel>)CheckEventBattlePass), (ICollection<IDisposable>)onNewStream);
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
			EventRewardAmplitudeEvent eventRewardAmplitudeEvent = new EventRewardAmplitudeEvent(uniqID, eventId, filteredRewardsCount, 0, 0);
			((IAnalyticSender<AmplitudeEvent>)(object)amplitude).AddEvent((AmplitudeEvent)eventRewardAmplitudeEvent);
		}
	}

	private IObservable<NotifyReward> OnAnyRewardUpdate(CalendarModel calendarModel)
	{
		return Observable.Where<NotifyReward>(Observable.TakeWhile<NotifyReward>(_eventSettingsProvider.GetEvent(calendarModel.BalanceId).OnRewardUpdate(), (Func<NotifyReward, bool>)((NotifyReward _) => _calendarQueue.IsCalendarActive(calendarModel))), (Func<NotifyReward, bool>)((NotifyReward _reward) => _reward.State.Value == EntityStatus.Rewarded));
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
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<bool>(Observable.Select<RewardWithManyConditions, bool>(battlePass.FreeRewardContainer.OnRewardUpdate(), (Func<RewardWithManyConditions, bool>)((RewardWithManyConditions reward) => reward.IsRewarded)), (Action<bool>)delegate
			{
				SendEventBattlePassRewardCollected(calendarId, eventId, battlePass);
			}), (ICollection<IDisposable>)onNewStream);
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<bool>(Observable.Select<RewardWithManyConditions, bool>(battlePass.PremiumRewardContainer.OnRewardUpdate(), (Func<RewardWithManyConditions, bool>)((RewardWithManyConditions reward) => reward.IsRewarded)), (Action<bool>)delegate
			{
				SendEventBattlePassRewardCollected(calendarId, eventId, battlePass);
			}), (ICollection<IDisposable>)onNewStream);
		}
	}

	private void SendEventBattlePassRewardCollected(int calendarId, int eventId, BattlePass battlePass)
	{
		int free_track = battlePass.FreeRewardContainer.Rewards.Count((RewardWithManyConditions reward) => reward.IsRewarded);
		int paid_track = battlePass.PremiumRewardContainer.Rewards.Count((RewardWithManyConditions reward) => reward.IsRewarded);
		EventRewardAmplitudeEvent eventRewardAmplitudeEvent = new EventRewardAmplitudeEvent(calendarId, eventId, 0, free_track, paid_track);
		((IAnalyticSender<AmplitudeEvent>)(object)amplitude).AddEvent((AmplitudeEvent)eventRewardAmplitudeEvent);
	}
}
