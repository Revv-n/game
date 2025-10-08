namespace GreenT.HornyScapes.Analytics;

public class BattlePassFreeRewardAmplitudeEvent : AmplitudeEvent
{
	private const string ANALYTIC_EVENT = "calendar_bp_reward_free";

	private const string CALENDAR_ID = "calendar_id";

	private const string BP_ID = "bp_id";

	private const string REWARDS_COUNT = "rewards_count";

	public BattlePassFreeRewardAmplitudeEvent(int calendarId, int bpId, int rewards_count)
		: base("calendar_bp_reward_free")
	{
		AddEventParams("calendar_id", calendarId);
		AddEventParams("bp_id", bpId);
		AddEventParams("rewards_count", rewards_count);
	}
}
