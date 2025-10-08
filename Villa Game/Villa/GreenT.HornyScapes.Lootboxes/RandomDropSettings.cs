using GreenT.HornyScapes.Analytics;
using GreenT.Types;
using StripClub.Model;
using StripClub.Model.Cards;
using Zenject;

namespace GreenT.HornyScapes.Lootboxes;

public class RandomDropSettings : DropSettings
{
	public int Weight { get; }

	public RandomDropSettings(RewType type, Selector selector, int quantity, int delta, ContentType contentType, int weight, CardsCollection cards, LinkedContentAnalyticDataFactory analyticDataFactory, IFactory<RewType, Selector, int, int, ContentType, LinkedContentAnalyticData, LinkedContent> linkedContentFactory)
		: base(type, selector, quantity, delta, contentType, cards, analyticDataFactory, linkedContentFactory)
	{
		Weight = weight;
	}
}
