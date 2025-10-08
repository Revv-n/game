using System;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Events;
using GreenT.Steam.Achievements.Goals.Objectives;
using Merge.Meta.RoomObjects;
using StripClub.NewEvent.Data;
using UniRx;

namespace GreenT.Steam.Achievements.Goals;

public class CollectAllEventRewards : InstantTrackService, IDisposable
{
	private readonly EventProvider _eventProvider;

	private IDisposable _disposable;

	public CollectAllEventRewards(AchievementService achievementService, AchievementDTO achievement, EventProvider eventProvider)
		: base(achievementService, achievement)
	{
		_eventProvider = eventProvider;
	}

	public override void Track()
	{
		if (IsComplete())
		{
			return;
		}
		EntityStatus[] condition = new EntityStatus[2]
		{
			EntityStatus.Blocked,
			EntityStatus.InProgress
		};
		_disposable = ObservableExtensions.Subscribe<(CalendarModel, Event)>(Observable.Where<(CalendarModel, Event)>(Observable.SelectMany<(CalendarModel, Event), (CalendarModel, Event)>(Observable.Where<(CalendarModel, Event)>(Observable.TakeWhile<(CalendarModel, Event)>((IObservable<(CalendarModel, Event)>)_eventProvider.CurrentCalendarProperty, (Func<(CalendarModel, Event), bool>)(((CalendarModel calendar, Event @event) _) => !IsComplete())), (Func<(CalendarModel, Event), bool>)(((CalendarModel calendar, Event @event) tuple) => tuple.@event != null)), (Func<(CalendarModel, Event), IObservable<(CalendarModel, Event)>>)(((CalendarModel calendar, Event @event) tuple) => Observable.Select<NotifyReward, (CalendarModel, Event)>(tuple.@event.OnRewardUpdate(), (Func<NotifyReward, (CalendarModel, Event)>)((NotifyReward _) => tuple)))), (Func<(CalendarModel, Event), bool>)(((CalendarModel calendar, Event @event) tuple) => tuple.@event.GetFilteredRewardsCount(condition) == 0)), (Action<(CalendarModel, Event)>)delegate
		{
			AchievementService.UnlockAchievement(Achievement);
		});
	}

	public void Dispose()
	{
		_disposable?.Dispose();
	}
}
