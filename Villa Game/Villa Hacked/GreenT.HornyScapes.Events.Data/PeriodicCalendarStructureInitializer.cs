using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Data;
using Zenject;

namespace GreenT.HornyScapes.Events.Data;

public class PeriodicCalendarStructureInitializer : StructureInitializerViaArray<PeriodicCalendarMapper, PeriodicCalendar, CalendarModel>
{
	public PeriodicCalendarStructureInitializer(IManager<CalendarModel> manager, IFactory<PeriodicCalendarMapper, PeriodicCalendar> factory, IEnumerable<IStructureInitializer> others = null)
		: base(manager, factory, others)
	{
	}

	public override IObservable<bool> Initialize(IEnumerable<PeriodicCalendarMapper> mappers)
	{
		try
		{
			IOrderedEnumerable<PeriodicCalendarMapper> mappers2 = mappers.OrderBy((PeriodicCalendarMapper mapper) => mapper.start_date);
			return base.Initialize((IEnumerable<PeriodicCalendarMapper>)mappers2);
		}
		catch (Exception exception)
		{
			throw exception.LogException();
		}
	}
}
