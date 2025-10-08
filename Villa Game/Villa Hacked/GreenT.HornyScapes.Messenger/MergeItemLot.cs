using GreenT.HornyScapes.MergeCore;
using Merge;
using UnityEngine;

namespace GreenT.HornyScapes.Messenger;

public class MergeItemLot : IItemLot
{
	private readonly GIKey _itemKey;

	public int TargetCount { get; }

	public Sprite Icon { get; }

	public MergeItemLot(GIKey itemKey, int count, Sprite icon)
	{
		_itemKey = itemKey;
		TargetCount = count;
		Icon = icon;
	}

	public int GetCurrentCount()
	{
		return GetPlayersItemCount();
	}

	public bool CheckIsEnough()
	{
		return GetCurrentCount() >= TargetCount;
	}

	public void Buy()
	{
		Controller<GameItemController>.Instance.RemoveItem(_itemKey, TargetCount);
	}

	private int GetPlayersItemCount()
	{
		return Mathf.Min(Controller<GameItemController>.Instance.CalcAvailableItems(_itemKey), TargetCount);
	}
}
