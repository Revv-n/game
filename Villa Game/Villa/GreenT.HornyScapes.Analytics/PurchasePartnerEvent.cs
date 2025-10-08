using GreenT.HornyScapes.Monetization;
using StripClub.Model.Shop;

namespace GreenT.HornyScapes.Analytics;

public class PurchasePartnerEvent : PartnerEvent
{
	private const string EVENT_NAME_KEY = "purchase";

	public PurchasePartnerEvent(PaymentIntentData data, Lot lot, object price)
		: base("purchase")
	{
		AddEventParams("itemId", AnalyticsExtensions.GetItemIdWithPostfix(data, lot));
		AddEventParams("price", price.ToString());
	}
}
