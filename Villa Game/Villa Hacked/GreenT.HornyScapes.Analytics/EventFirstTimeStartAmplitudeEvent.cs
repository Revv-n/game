namespace GreenT.HornyScapes.Analytics;

public class EventFirstTimeStartAmplitudeEvent : AmplitudeEvent
{
	private const string ANALYTIC_EVENT = "calendar_event_firststart";

	private const string CALENDAR_ID = "calendar_id";

	private const string EVENT_ID = "event_id";

	public EventFirstTimeStartAmplitudeEvent(int calendarId, int eventId)
		: base("calendar_event_firststart")
	{
		((AnalyticsEvent)this).AddEventParams("calendar_id", (object)calendarId);
		((AnalyticsEvent)this).AddEventParams("event_id", (object)eventId);
	}
}
