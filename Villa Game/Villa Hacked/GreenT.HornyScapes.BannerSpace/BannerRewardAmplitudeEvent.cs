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
		((AnalyticsEvent)this).AddEventParams("source", (object)source);
		((AnalyticsEvent)this).AddEventParams("lootbox_id", (object)lootboxId);
		((AnalyticsEvent)this).AddEventParams("garant_id", (object)garantId);
		((AnalyticsEvent)this).AddEventParams("garant_id_step", (object)garantIdStep);
		((AnalyticsEvent)this).AddEventParams("garant_chance", (object)garantChance);
		((AnalyticsEvent)this).AddEventParams("currency_type", (object)currencyType);
		((AnalyticsEvent)this).AddEventParams("price", (object)price);
	}
}
