using System.Collections.Generic;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Data;
using Zenject;

namespace GreenT.HornyScapes.Events.Data;

public class MiniEventCalendarStructureInitializer : StructureInitializerViaArray<MiniEventCalendarMapper, PeriodicCalendar, CalendarModel>
{
	public MiniEventCalendarStructureInitializer(IManager<CalendarModel> manager, MiniEventCalendarFactory factory, IEnumerable<IStructureInitializer> others = null)
		: base(manager, (IFactory<MiniEventCalendarMapper, PeriodicCalendar>)(object)factory, others)
	{
	}
}
