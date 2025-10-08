using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.Sellouts.Models;

namespace GreenT.HornyScapes.BattlePassSpace;

public sealed class SelloutStateAnalytic : BaseEventStateAnalytic<Sellout>
{
	public SelloutStateAnalytic(SelloutStateService eventStateService, EventAnalytic eventAnalytic, SelloutCalendarLoader calendarLoader)
		: base((BaseStateService<Sellout>)eventStateService, eventAnalytic, (ICalendarLoader)calendarLoader)
	{
	}
}
