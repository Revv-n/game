namespace GreenT.HornyScapes.Analytics;

public class BattlePassFirstPushAmplitudeEvent : AmplitudeEvent
{
	private const string ANALYTIC_EVENT = "calendar_bp_popup";

	private const string CALENDAR_ID = "calendar_id";

	private const string BP_ID = "bp_id";

	public BattlePassFirstPushAmplitudeEvent(int calendarId, int bpId)
		: base("calendar_bp_popup")
	{
		((AnalyticsEvent)this).AddEventParams("calendar_id", (object)calendarId);
		((AnalyticsEvent)this).AddEventParams("bp_id", (object)bpId);
	}
}
