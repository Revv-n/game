using GreenT.HornyScapes.Analytics;

namespace GreenT.HornyScapes.Relationships.Analytics;

public class RelationshipPointsEvent : AmplitudeEvent
{
	private const string EventTypeKey = "grind_lovepoints";

	private const string RewardKey = "id";

	public RelationshipPointsEvent(int rewardId)
		: base("grind_lovepoints")
	{
		((AnalyticsEvent)this).AddEventParams("id", (object)rewardId);
	}
}
