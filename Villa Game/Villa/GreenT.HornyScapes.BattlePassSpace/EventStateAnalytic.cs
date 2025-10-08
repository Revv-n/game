using GreenT.HornyScapes.Events;

namespace GreenT.HornyScapes.BattlePassSpace;

public sealed class EventStateAnalytic : BaseEventStateAnalytic<Event>
{
	public EventStateAnalytic(EventsStateService eventStateService, EventAnalytic eventAnalytic, EventCalendarLoader calendarLoader)
		: base((BaseStateService<Event>)eventStateService, eventAnalytic, (ICalendarLoader)calendarLoader)
	{
	}
}
