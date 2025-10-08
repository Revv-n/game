using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Analytics;
using StripClub.Model.Data;

namespace GreenT.HornyScapes.MergeStore;

public sealed class RefreshAmplitudeEvent : AmplitudeEvent
{
	private const string EventKey = "item_shop_refresh";

	private const string ForceUpdate = "force_update";

	private const string CurrencyType = "currency_type";

	private const string ContentType = "content_type";

	private const string ID = "set_id";

	private const string Price = "price";

	private const string Items = "items";

	public RefreshAmplitudeEvent(Cost cost, StoreSection section)
		: base("item_shop_refresh")
	{
		string value = ((cost.Amount <= 0) ? "Time" : cost.Currency.ToString());
		List<string> value2 = (from x in section.GetItems()
			select x.ItemKey.ToString()).ToList();
		AddEventParams("set_id", section.ID);
		AddEventParams("content_type", section.ContentType);
		AddEventParams("items", value2);
		AddEventParams("force_update", cost.Amount != 0);
		AddEventParams("currency_type", value);
		AddEventParams("price", cost.Amount);
	}
}
