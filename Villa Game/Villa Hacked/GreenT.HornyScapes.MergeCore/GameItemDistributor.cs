using Merge;

namespace GreenT.HornyScapes.MergeCore;

public class GameItemDistributor
{
	private readonly GameItemFactory itemFactory;

	public GameItemDistributor(GameItemFactory itemFactory)
	{
		this.itemFactory = itemFactory;
	}

	public GameItem AddItem(GIData giData, MergeField field)
	{
		GameItem result = itemFactory.Create(giData, field);
		field.Data.GameItems.Add(giData);
		return result;
	}
}
