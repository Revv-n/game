namespace GreenT.HornyScapes.Analytics;

public sealed class MiniEventFirstTimeSeenAmplitudeEvent : AmplitudeEvent
{
	private const string ANALYTIC_EVENT = "calendar_miniEvent_firststart";

	private const string CALENDAR_ID = "calendar_id";

	private const string MINIEVENT_ID = "miniEvent_id";

	public MiniEventFirstTimeSeenAmplitudeEvent(int calendarId, int minieventId)
		: base("calendar_miniEvent_firststart")
	{
		((AnalyticsEvent)this).AddEventParams("miniEvent_id", (object)minieventId);
		((AnalyticsEvent)this).AddEventParams("calendar_id", (object)calendarId);
	}
}
