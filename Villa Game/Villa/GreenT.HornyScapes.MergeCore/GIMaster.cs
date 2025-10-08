using System;
using Merge;

namespace GreenT.HornyScapes.MergeCore;

public static class GIMaster
{
	public static GameItemController Field => Controller<GameItemController>.Instance;

	public static event Action<GameItem, GameItem, MergeField> OnSwap;

	public static bool TryGetItemAt(Point pnt, out GameItem result)
	{
		result = Field.CurrentField.Field[pnt];
		return Field.CurrentField.Field[pnt] != null;
	}

	public static GameItem GetItemAt(Point pnt)
	{
		return Field.CurrentField.Field[pnt];
	}

	public static GameItem SwapItem(MergeField field, GameItem oldItem, GIData newItemData)
	{
		Field.RemoveItem(oldItem, field);
		GameItem gameItem = Field.CreateItem(newItemData, field);
		GIMaster.OnSwap?.Invoke(oldItem, gameItem, field);
		return gameItem;
	}
}
