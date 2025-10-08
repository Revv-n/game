using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.MiniEvents;

namespace GreenT.HornyScapes.BattlePassSpace;

public sealed class MiniEventStateAnalytic : BaseEventStateAnalytic<MiniEvent>
{
	public MiniEventStateAnalytic(MiniEventsStateService eventStateService, EventAnalytic eventAnalytic, MiniEventCalendarLoader calendarLoader)
		: base((BaseStateService<MiniEvent>)eventStateService, eventAnalytic, (ICalendarLoader)calendarLoader)
	{
	}
}
