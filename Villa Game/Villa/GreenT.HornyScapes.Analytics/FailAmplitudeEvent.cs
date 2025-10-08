namespace GreenT.HornyScapes.Analytics;

public class FailAmplitudeEvent : AmplitudeEvent
{
	private const string EVENT_NAME_KEY = "fail_payment";

	private const string REASON_KEY = "reason";

	public FailAmplitudeEvent(string reason)
		: base("fail_payment")
	{
		AddEventParams("reason", reason);
	}
}
