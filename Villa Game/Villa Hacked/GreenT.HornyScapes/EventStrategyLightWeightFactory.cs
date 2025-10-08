using GreenT.HornyScapes.Events;
using StripClub.NewEvent.Data;

namespace GreenT.HornyScapes;

public sealed class EventStrategyLightWeightFactory : CalendarStrategyLightWeightFactory<EventCalendarStrategy, EventDataBuilder, EventCalendarLoader, EventCalendarDispenser, EventDataCleaner>
{
	public EventStrategyLightWeightFactory(EventDataBuilder builder, EventCalendarLoader loader, EventCalendarDispenser dispenser, EventDataCleaner cleaner)
		: base(builder, loader, dispenser, cleaner)
	{
	}

	protected override EventCalendarStrategy CreateLightWeight()
	{
		return new EventCalendarStrategy(_builder, _loader, _dispenser, _cleaner);
	}
}
