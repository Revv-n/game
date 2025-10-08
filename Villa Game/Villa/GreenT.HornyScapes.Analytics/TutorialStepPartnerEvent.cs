namespace GreenT.HornyScapes.Analytics;

public class TutorialStepPartnerEvent : PartnerEvent
{
	private const string EVENT_NAME_KEY = "tutor";

	public TutorialStepPartnerEvent(int stepId)
		: base("tutor")
	{
		AddEventParams("tutor", stepId.ToString());
	}
}
