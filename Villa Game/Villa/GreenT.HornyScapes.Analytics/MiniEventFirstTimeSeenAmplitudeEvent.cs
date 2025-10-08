namespace GreenT.HornyScapes.Analytics;

public sealed class MiniEventFirstTimeSeenAmplitudeEvent : AmplitudeEvent
{
	private const string ANALYTIC_EVENT = "calendar_miniEvent_firststart";

	private const string CALENDAR_ID = "calendar_id";

	private const string MINIEVENT_ID = "miniEvent_id";

	public MiniEventFirstTimeSeenAmplitudeEvent(int calendarId, int minieventId)
		: base("calendar_miniEvent_firststart")
	{
		AddEventParams("miniEvent_id", minieventId);
		AddEventParams("calendar_id", calendarId);
	}
}
