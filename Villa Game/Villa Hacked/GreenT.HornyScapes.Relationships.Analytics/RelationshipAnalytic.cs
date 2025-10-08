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
		RelationshipPointsEvent relationshipPointsEvent = new RelationshipPointsEvent(rewardId);
		((IAnalyticSender<AmplitudeEvent>)(object)amplitude).AddEvent((AmplitudeEvent)relationshipPointsEvent);
	}

	public void SendRewardReceivedEvent(int relationshipId)
	{
		RelationshipRewardEvent relationshipRewardEvent = new RelationshipRewardEvent(relationshipId);
		((IAnalyticSender<AmplitudeEvent>)(object)amplitude).AddEvent((AmplitudeEvent)relationshipRewardEvent);
	}
}
