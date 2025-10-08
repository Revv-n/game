using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.Events;

namespace GreenT.HornyScapes;

public class EventEndEvent : AmplitudeEvent
{
	private const string EventTypeKey = "event_end";

	private const string CurrentEventTypeKey = "event_type";

	private const string EventIdKey = "event_id";

	private const string CalendarIdKey = "calendar_id";

	private const string DurationKey = "duration";

	public EventEndEvent(EventStructureType currentEventType, int eventId, int calendarId, long duration)
		: base("event_end")
	{
		AddEventParams("event_type", currentEventType);
		AddEventParams("event_id", eventId);
		AddEventParams("calendar_id", calendarId);
		AddEventParams("duration", duration);
	}
}
