using GreenT.HornyScapes.BattlePassSpace;
using Merge.Meta.RoomObjects;
using StripClub.Model;
using UniRx;

namespace GreenT.HornyScapes;

public abstract class EventIntervalLocker : Locker
{
	protected readonly long from;

	protected readonly long to;

	private CalendarModel calendarModel;

	public EventIntervalLocker(long from, long to)
	{
		this.from = from;
		this.to = to;
	}

	public void Set(CalendarModel calendarModel)
	{
		this.calendarModel = calendarModel;
	}

	public void Set(long currentTime)
	{
		HandleLocker(currentTime);
	}

	private void HandleLocker(long currentTime)
	{
		if (calendarModel != null)
		{
			ReactiveProperty<EntityStatus> calendarState = calendarModel.CalendarState;
			if (calendarState != null && calendarState.Value == EntityStatus.InProgress)
			{
				long num = currentTime - calendarModel.StartedTimeStamp;
				SetIsOpen(num);
				return;
			}
		}
		isOpen.Value = false;
	}

	protected abstract void SetIsOpen(long time);
}
