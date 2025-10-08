using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.GameItems;
using GreenT.HornyScapes.MergeCore;
using StripClub.Model;
using Zenject;

namespace GreenT.HornyScapes.Content;

public class MergeItemContentFactory : IFactory<int, int, LinkedContentAnalyticData, MergeItemLinkedContent>, IFactory, IFactory<int, int, LinkedContentAnalyticData, LinkedContent, MergeItemLinkedContent>
{
	private IMergeIconProvider iconManager;

	private GameItemConfigManager gameItemConfigManager;

	public MergeItemContentFactory(IMergeIconProvider iconManager, GameItemConfigManager gameItemConfigManager)
	{
		this.iconManager = iconManager;
		this.gameItemConfigManager = gameItemConfigManager;
	}

	public MergeItemLinkedContent Create(int itemID, int count, LinkedContentAnalyticData analyticData, LinkedContent nestedContent = null)
	{
		gameItemConfigManager.TryGetConfig(itemID, out var giConfig);
		return new MergeItemLinkedContent(iconManager, giConfig, count, analyticData, nestedContent);
	}

	public MergeItemLinkedContent Create(int itemID, int count, LinkedContentAnalyticData analyticData)
	{
		return Create(itemID, count, analyticData, null);
	}
}
