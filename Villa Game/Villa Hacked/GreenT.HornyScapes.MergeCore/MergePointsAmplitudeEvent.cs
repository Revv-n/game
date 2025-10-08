using System.Collections.Generic;
using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.Lootboxes;
using Merge;

namespace GreenT.HornyScapes.MergeCore;

public sealed class MergePointsAmplitudeEvent : AmplitudeEvent
{
	private const string EventKey = "currency_merge_points_received";

	private const string CurrencyType = "currency_type";

	private const string ItemSource = "item_source";

	private const string ID = "id";

	private const string Amount = "amount";

	private const string Item = "merge_item";

	private static readonly Dictionary<MergePointsCreatType, string> SourceValues = new Dictionary<MergePointsCreatType, string>
	{
		{
			MergePointsCreatType.Merge,
			"merge"
		},
		{
			MergePointsCreatType.Bubble,
			"bubble"
		}
	};

	public MergePointsAmplitudeEvent(GameItem gameItem, int count, CurrencySelector selector, MergePointsCreatType analyticType)
		: base("currency_merge_points_received")
	{
		string valueOrDefault = CollectionExtensions.GetValueOrDefault<MergePointsCreatType, string>((IReadOnlyDictionary<MergePointsCreatType, string>)SourceValues, analyticType, "non");
		((AnalyticsEvent)this).AddEventParams("currency_type", (object)selector.Currency);
		((AnalyticsEvent)this).AddEventParams("item_source", (object)valueOrDefault);
		((AnalyticsEvent)this).AddEventParams("id", (object)selector.Identificator[0]);
		((AnalyticsEvent)this).AddEventParams("amount", (object)count);
		((AnalyticsEvent)this).AddEventParams("merge_item", (object)gameItem.Key.ToString());
	}
}
