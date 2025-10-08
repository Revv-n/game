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
		string text = ((cost.Amount <= 0) ? "Time" : cost.Currency.ToString());
		List<string> list = (from x in section.GetItems()
			select x.ItemKey.ToString()).ToList();
		((AnalyticsEvent)this).AddEventParams("set_id", (object)section.ID);
		((AnalyticsEvent)this).AddEventParams("content_type", (object)section.ContentType);
		((AnalyticsEvent)this).AddEventParams("items", (object)list);
		((AnalyticsEvent)this).AddEventParams("force_update", (object)(cost.Amount != 0));
		((AnalyticsEvent)this).AddEventParams("currency_type", (object)text);
		((AnalyticsEvent)this).AddEventParams("price", (object)cost.Amount);
	}
}
