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
		AddEventParams("monetization_id", lot.MonetizationID);
		AddEventParams("currency_type_", lot.Price.Currency);
		AddEventParams("source", lot.ShopSource);
	}
}
