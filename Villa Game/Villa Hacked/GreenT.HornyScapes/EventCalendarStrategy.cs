using GreenT.HornyScapes.Events;
using StripClub.NewEvent.Data;

namespace GreenT.HornyScapes;

public sealed class EventCalendarStrategy : DefaultCalendarStrategy<EventDataBuilder, EventCalendarLoader, EventCalendarDispenser, EventDataCleaner>
{
	public EventCalendarStrategy(EventDataBuilder builder, EventCalendarLoader loader, EventCalendarDispenser dispenser, EventDataCleaner eventDataCleaner)
		: base(builder, loader, dispenser, eventDataCleaner)
	{
	}
}
