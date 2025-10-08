namespace GreenT.HornyScapes.Analytics;

public class FailPaymentEvent : PartnerEvent
{
	private const string EVENT_NAME_KEY = "fail_payment";

	public FailPaymentEvent(object failReason)
		: base("fail_payment")
	{
		((AnalyticsEvent)this).AddEventParams("fail_reason", failReason);
	}
}
