using StripClub.Model.Shop;

namespace GreenT.HornyScapes.Analytics.Bank;

public class SummonAmplitudeEvent : BaseSummonAmplitudeEvent
{
	private const string EVENT_NAME = "summon";

	private const string VALUABLE_SUMMON = "valuable_summon";

	private const string FREE_SUMMON = "free_summon";

	private const string SUMMON_TYPE = "summon_type";

	private const string ID_KEY = "id";

	private const string CURRENCY_KEY = "currency_type";

	private const string PRICE_KEY = "price";

	private const string SHOP_SOURCE = "source";

	public SummonAmplitudeEvent(SummonLot summonLot)
		: base("summon")
	{
		((AnalyticsEvent)this).AddEventParams("source", (object)summonLot.ShopSource);
		((AnalyticsEvent)this).AddEventParams("summon_type", (object)((summonLot.Price.Value > 0) ? "valuable_summon" : "free_summon"));
		((AnalyticsEvent)this).AddEventParams("id", (object)summonLot.ID);
		if (summonLot.Price.Value != 0)
		{
			((AnalyticsEvent)this).AddEventParams("currency_type", (object)summonLot.Price.Currency);
			((AnalyticsEvent)this).AddEventParams("price", (object)summonLot.Price.Value);
		}
	}
}
