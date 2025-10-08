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
		_disposable = (from tuple in (from tuple in _eventProvider.CurrentCalendarProperty.TakeWhile(((CalendarModel calendar, Event @event) _) => !IsComplete())
				where tuple.@event != null
				select tuple).SelectMany(((CalendarModel calendar, Event @event) tuple) => from _ in tuple.@event.OnRewardUpdate()
				select tuple)
			where tuple.@event.GetFilteredRewardsCount(condition) == 0
			select tuple).Subscribe(delegate
		{
			AchievementService.UnlockAchievement(Achievement);
		});
	}

	public void Dispose()
	{
		_disposable?.Dispose();
	}
}
