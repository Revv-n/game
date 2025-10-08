namespace GreenT.HornyScapes.Analytics;

public class PaymentCheckPartnerEvent : PartnerEvent
{
	private const string EVENT_NAME_KEY = "payment_check";

	public PaymentCheckPartnerEvent(object itemUniqId, object price, int checkoutAttemptCount)
		: base("payment_check")
	{
		((AnalyticsEvent)this).AddEventParams("itemId", (object)itemUniqId.ToString());
		((AnalyticsEvent)this).AddEventParams("price", (object)price.ToString());
		((AnalyticsEvent)this).AddEventParams("attempt", (object)checkoutAttemptCount.ToString());
	}
}
