namespace GreenT.HornyScapes.Analytics;

public class BattlePassFirstStartAmplitudeEvent : AmplitudeEvent
{
	private const string ANALYTIC_EVENT = "calendar_bp_firststart";

	private const string CALENDAR_ID = "calendar_id";

	private const string BP_ID = "bp_id";

	public BattlePassFirstStartAmplitudeEvent(int calendarId, int bpId)
		: base("calendar_bp_firststart")
	{
		AddEventParams("calendar_id", calendarId);
		AddEventParams("bp_id", bpId);
	}
}
