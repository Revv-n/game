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
		AddEventParams("source", summonLot.ShopSource);
		AddEventParams("summon_type", (summonLot.Price.Value > 0) ? "valuable_summon" : "free_summon");
		AddEventParams("id", summonLot.ID);
		if (summonLot.Price.Value != 0)
		{
			AddEventParams("currency_type", summonLot.Price.Currency);
			AddEventParams("price", summonLot.Price.Value);
		}
	}
}
