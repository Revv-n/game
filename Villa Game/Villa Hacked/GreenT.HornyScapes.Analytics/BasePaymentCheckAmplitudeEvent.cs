using GreenT.HornyScapes.Monetization;

namespace GreenT.HornyScapes.Analytics;

public class BasePaymentCheckAmplitudeEvent : AmplitudeEvent
{
	private const string EVENT_NAME_KEY = "payment_check";

	private const string ITEM_ID_KEY = "monetization_id";

	private const string PRICE_KEY = "price";

	private const string CHECKOUT_KEY = "attempt";

	public BasePaymentCheckAmplitudeEvent(Product product, int checkoutAttemptCount)
		: base("payment_check")
	{
		((AnalyticsEvent)this).AddEventParams("payment_check", (object)product.LotID);
		((AnalyticsEvent)this).AddEventParams("monetization_id", (object)product.ItemId);
		((AnalyticsEvent)this).AddEventParams("price", (object)product.Price);
		((AnalyticsEvent)this).AddEventParams("attempt", (object)checkoutAttemptCount);
	}
}
