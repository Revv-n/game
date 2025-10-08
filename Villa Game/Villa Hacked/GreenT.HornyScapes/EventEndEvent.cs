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
		((AnalyticsEvent)this).AddEventParams("event_type", (object)currentEventType);
		((AnalyticsEvent)this).AddEventParams("event_id", (object)eventId);
		((AnalyticsEvent)this).AddEventParams("calendar_id", (object)calendarId);
		((AnalyticsEvent)this).AddEventParams("duration", (object)duration);
	}
}
