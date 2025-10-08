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
		string valueOrDefault = SourceValues.GetValueOrDefault(analyticType, "non");
		AddEventParams("currency_type", selector.Currency);
		AddEventParams("item_source", valueOrDefault);
		AddEventParams("id", selector.Identificator[0]);
		AddEventParams("amount", count);
		AddEventParams("merge_item", gameItem.Key.ToString());
	}
}
