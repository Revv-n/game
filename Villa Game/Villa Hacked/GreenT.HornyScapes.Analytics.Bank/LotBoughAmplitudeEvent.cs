using StripClub.Model.Shop;

namespace GreenT.HornyScapes.Analytics.Bank;

public class LotBoughAmplitudeEvent : AmplitudeEvent
{
	private const string EventKey = "lot_bought";

	private const string MonetizationId = "monetization_id";

	private const string CurrencyType = "currency_type_";

	private const string Source = "source";

	public LotBoughAmplitudeEvent(ValuableLot<decimal> lot)
		: base("lot_bought")
	{
		((AnalyticsEvent)this).AddEventParams("monetization_id", (object)lot.MonetizationID);
		((AnalyticsEvent)this).AddEventParams("currency_type_", (object)lot.Price.Currency);
		((AnalyticsEvent)this).AddEventParams("source", (object)lot.ShopSource);
	}
}
