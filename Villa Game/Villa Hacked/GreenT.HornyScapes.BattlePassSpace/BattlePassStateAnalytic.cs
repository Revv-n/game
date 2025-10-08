using GreenT.HornyScapes.Events;

namespace GreenT.HornyScapes.BattlePassSpace;

public sealed class BattlePassStateAnalytic : BaseEventStateAnalytic<BattlePass>
{
	public BattlePassStateAnalytic(BattlePassStateService eventStateService, EventAnalytic eventAnalytic, BattlePassCalendarLoader calendarLoader)
		: base((BaseStateService<BattlePass>)eventStateService, eventAnalytic, (ICalendarLoader)calendarLoader)
	{
	}
}
