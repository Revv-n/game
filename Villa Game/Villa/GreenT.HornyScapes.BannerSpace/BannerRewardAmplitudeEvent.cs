using GreenT.HornyScapes.Analytics;
using StripClub.Model;

namespace GreenT.HornyScapes.BannerSpace;

public sealed class BannerRewardAmplitudeEvent : AmplitudeEvent
{
	private const string EventKey = "banner_reward";

	private const string Source = "source";

	private const string LootboxId = "lootbox_id";

	private const string GarantId = "garant_id";

	private const string GarantIdStep = "garant_id_step";

	private const string GarantChance = "garant_chance";

	private const string Price = "price";

	private const string CurrencyType = "currency_type";

	public BannerRewardAmplitudeEvent(string source, int lootboxId, int garantId, int garantIdStep, int garantChance, int price, CurrencyType currencyType)
		: base("banner_reward")
	{
		AddEventParams("source", source);
		AddEventParams("lootbox_id", lootboxId);
		AddEventParams("garant_id", garantId);
		AddEventParams("garant_id_step", garantIdStep);
		AddEventParams("garant_chance", garantChance);
		AddEventParams("currency_type", currencyType);
		AddEventParams("price", price);
	}
}
