namespace GreenT.HornyScapes.Analytics;

public class EventFirstTimePushAmplitudeEvent : AmplitudeEvent
{
	private const string ANALYTIC_EVENT = "calendar_event_popup";

	private const string CALENDAR_ID = "calendar_id";

	private const string EVENT_ID = "event_id";

	public EventFirstTimePushAmplitudeEvent(int calendarId, int eventId)
		: base("calendar_event_popup")
	{
		((AnalyticsEvent)this).AddEventParams("calendar_id", (object)calendarId);
		((AnalyticsEvent)this).AddEventParams("event_id", (object)eventId);
	}
}
