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
		AddEventParams("section", request.Item.Section);
		AddEventParams("currency_type", cost.Currency);
		AddEventParams("price", cost.Amount);
		AddEventParams("item", request.Item.ItemKey);
		AddEventParams("bought_position", request.ButtonPosition);
		AddEventParams("set_id", request.Section.ID);
	}
}
