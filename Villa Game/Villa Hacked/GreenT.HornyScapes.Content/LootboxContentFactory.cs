using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.Lootboxes;
using StripClub.Model;
using Zenject;

namespace GreenT.HornyScapes.Content;

public class LootboxContentFactory : IFactory<int, LinkedContentAnalyticData, LootboxLinkedContent>, IFactory, IFactory<int, LinkedContentAnalyticData, LinkedContent, LootboxLinkedContent>
{
	private readonly LootboxCollection lootboxCollection;

	private readonly ILootboxOpener lootboxOpener;

	public LootboxContentFactory(LootboxCollection lootboxCollection, ILootboxOpener lootboxOpener)
	{
		this.lootboxCollection = lootboxCollection;
		this.lootboxOpener = lootboxOpener;
	}

	public LootboxLinkedContent Create(int lootboxID, LinkedContentAnalyticData analyticData, LinkedContent nestedContent = null)
	{
		return new LootboxLinkedContent(lootboxCollection.Get(lootboxID), lootboxOpener, analyticData, nestedContent);
	}

	public LootboxLinkedContent Create(int lootboxID, LinkedContentAnalyticData analyticData)
	{
		return Create(lootboxID, analyticData, null);
	}
}
