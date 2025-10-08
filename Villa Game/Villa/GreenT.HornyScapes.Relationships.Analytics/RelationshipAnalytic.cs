using GreenT.HornyScapes.Analytics;

namespace GreenT.HornyScapes.Relationships.Analytics;

public class RelationshipAnalytic : BaseAnalytic
{
	public RelationshipAnalytic(IAmplitudeSender<AmplitudeEvent> amplitude)
		: base(amplitude)
	{
	}

	public void SendPointsReceivedEvent(int rewardId)
	{
		RelationshipPointsEvent analyticsEvent = new RelationshipPointsEvent(rewardId);
		amplitude.AddEvent(analyticsEvent);
	}

	public void SendRewardReceivedEvent(int relationshipId)
	{
		RelationshipRewardEvent analyticsEvent = new RelationshipRewardEvent(relationshipId);
		amplitude.AddEvent(analyticsEvent);
	}
}
