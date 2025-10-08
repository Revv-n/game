namespace GreenT.HornyScapes.Analytics;

public interface IMarketingEventSender
{
	void SendTutorStepEvent(int tutorStepNumber);

	void SendPlayButtonEvent();
}
