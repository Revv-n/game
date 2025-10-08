using GreenT.HornyScapes.Analytics;

namespace GreenT.HornyScapes.Relationships.Analytics;

public class RelationshipRewardEvent : AmplitudeEvent
{
	private const string EventTypeKey = "reward_lovepoints_received";

	private const string RelationshipKey = "id";

	public RelationshipRewardEvent(int relationshipId)
		: base("reward_lovepoints_received")
	{
		((AnalyticsEvent)this).AddEventParams("id", (object)relationshipId);
	}
}
