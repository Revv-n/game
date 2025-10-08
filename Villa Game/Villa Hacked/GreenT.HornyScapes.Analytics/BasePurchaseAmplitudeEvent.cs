namespace GreenT.HornyScapes.Analytics;

public abstract class BasePurchaseAmplitudeEvent : AmplitudeEvent
{
	protected const string EVENT_NAME_KEY = "purchase_full";

	protected const string FIRST_EVENT_KEY = "first_purchase";

	protected const string BUNDLE_KEY = "bundle_key";

	protected const string PAYMENT_PRICE = "price";

	protected const string MONETIZATION_ID = "monetization_id";

	protected const string SHOP_SOURCE = "source";

	protected const string REGION = "region";

	protected const string SELLOUT_POINTS = "sellout_points";

	protected const string ADDITIONAL_STARSHOP = "on_starshop_id";

	protected const string COHORT = "cohort";

	protected BasePurchaseAmplitudeEvent(bool firstPurchase, decimal price, string monetizationId, string shopSource, decimal cohort, string region, int selloutPoints)
		: base("purchase_full")
	{
		((AnalyticsEvent)this).AddEventParams("first_purchase", (object)firstPurchase);
		((AnalyticsEvent)this).AddEventParams("price", (object)price);
		((AnalyticsEvent)this).AddEventParams("monetization_id", (object)monetizationId);
		((AnalyticsEvent)this).AddEventParams("source", (object)shopSource);
		((AnalyticsEvent)this).AddEventParams("region", (object)region);
		((AnalyticsEvent)this).AddEventParams("sellout_points", (object)selloutPoints);
	}

	protected BasePurchaseAmplitudeEvent(string event_key)
		: base(event_key)
	{
	}

	public void AddStarShopInfo(int starShopId)
	{
		((AnalyticsEvent)this).AddEventParams("on_starshop_id", (object)starShopId);
	}

	protected void AddBundleKey(string bundleKey)
	{
		((AnalyticsEvent)this).AddEventParams("bundle_key", (object)bundleKey);
	}

	protected string ConvertToIntervalString((int, int) interval)
	{
		return interval.Item1 + "-" + interval.Item2;
	}

	protected abstract (int, int) RoundCohortInterval(decimal cohort);
}
