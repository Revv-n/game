using GreenT.HornyScapes.Monetization;
using StripClub.Model.Shop;

namespace GreenT.HornyScapes.Analytics;

public class PurchasePartnerEvent : PartnerEvent
{
	private const string EVENT_NAME_KEY = "purchase";

	public PurchasePartnerEvent(PaymentIntentData data, Lot lot, object price)
		: base("purchase")
	{
		((AnalyticsEvent)this).AddEventParams("itemId", (object)AnalyticsExtensions.GetItemIdWithPostfix(data, lot));
		((AnalyticsEvent)this).AddEventParams("price", (object)price.ToString());
	}
}
