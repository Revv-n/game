using GreenT.HornyScapes.Analytics;

namespace GreenT.HornyScapes.Sellouts.Analytics;

public class SelloutRewardEvent : AmplitudeEvent
{
	private const string SelloutRewardKey = "sellout_reward";

	private const string SelloutIdKey = "sellout_id";

	private const string RewardsCountKey = "rewards_count";

	public SelloutRewardEvent(int selloutId, int rewardsCount)
		: base("sellout_reward")
	{
		AddEventParams("sellout_id", selloutId);
		AddEventParams("rewards_count", rewardsCount);
	}
}
