namespace GreenT.HornyScapes.Analytics;

public class BattlePassPremiumRewardAmplitudeEvent : AmplitudeEvent
{
	private const string ANALYTIC_EVENT = "calendar_bp_reward_premium";

	private const string CALENDAR_ID = "calendar_id";

	private const string BP_ID = "bp_id";

	private const string REWARDS_COUNT = "rewards_count";

	public BattlePassPremiumRewardAmplitudeEvent(int calendarId, int bpId, int rewards_count)
		: base("calendar_bp_reward_premium")
	{
		((AnalyticsEvent)this).AddEventParams("calendar_id", (object)calendarId);
		((AnalyticsEvent)this).AddEventParams("bp_id", (object)bpId);
		((AnalyticsEvent)this).AddEventParams("rewards_count", (object)rewards_count);
	}
}
