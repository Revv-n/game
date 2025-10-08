namespace GreenT.HornyScapes.Analytics;

public class PaymentCheckPartnerEvent : PartnerEvent
{
	private const string EVENT_NAME_KEY = "payment_check";

	public PaymentCheckPartnerEvent(object itemUniqId, object price, int checkoutAttemptCount)
		: base("payment_check")
	{
		AddEventParams("itemId", itemUniqId.ToString());
		AddEventParams("price", price.ToString());
		AddEventParams("attempt", checkoutAttemptCount.ToString());
	}
}
