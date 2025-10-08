using System.Collections.Generic;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Data;
using Zenject;

namespace GreenT.HornyScapes.Events.Data;

public class NoviceCalendarStructureInitializer : StructureInitializerViaArray<NoviceCalendarMapper, NoviceCalendar, CalendarModel>
{
	public NoviceCalendarStructureInitializer(IManager<CalendarModel> manager, IFactory<NoviceCalendarMapper, NoviceCalendar> factory, IEnumerable<IStructureInitializer> others = null)
		: base(manager, factory, others)
	{
	}
}
