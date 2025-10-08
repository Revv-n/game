using GreenT.HornyScapes.Analytics;
using StripClub.Model.Data;

namespace GreenT.HornyScapes.MergeStore;

public sealed class ItemShopBoughtAmplitudeEvent : AmplitudeEvent
{
	private const string EventKey = "item_shop_bought";

	private const string CurrencyType = "currency_type";

	private const string Price = "price";

	private const string Items = "item";

	private const string ID = "set_id";

	private const string Section = "section";

	private const string BoughtPosition = "bought_position";

	public ItemShopBoughtAmplitudeEvent(Cost cost, ItemBuyRequest request)
		: base("item_shop_bought")
	{
		((AnalyticsEvent)this).AddEventParams("section", (object)request.Item.Section);
		((AnalyticsEvent)this).AddEventParams("currency_type", (object)cost.Currency);
		((AnalyticsEvent)this).AddEventParams("price", (object)cost.Amount);
		((AnalyticsEvent)this).AddEventParams("item", (object)request.Item.ItemKey);
		((AnalyticsEvent)this).AddEventParams("bought_position", (object)request.ButtonPosition);
		((AnalyticsEvent)this).AddEventParams("set_id", (object)request.Section.ID);
	}
}
