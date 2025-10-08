using GreenT.HornyScapes.Events;
using Merge.Meta.RoomObjects;
using UniRx;

namespace GreenT.HornyScapes.External.StripClub._Scripts.NewEventScripts;

public class EventStateService
{
	private readonly CalendarQueue _calendarQueue;

	public bool HaveActiveEvent => _calendarQueue.HasActiveCalendar(EventStructureType.Event);

	public IReadOnlyReactiveProperty<bool> OnEventStateChange(EntityStatus state)
	{
		return ReactivePropertyExtensions.ToReactiveProperty<bool>(_calendarQueue.OnCalendarStateChange(EventStructureType.Event, state));
	}

	public EventStateService(CalendarQueue calendarQueue)
	{
		_calendarQueue = calendarQueue;
	}
}
