using System;
using System.Linq;
using Merge;
using UnityEngine;

namespace GreenT.HornyScapes.Cheats;

public class CheatMergeMergeMechanics : CheateMerge
{
	private void Update()
	{
		if (inputSetting.MergeAllAvailableItems.IsPressedKeys)
		{
			MergeAllAvailableItems();
		}
		if (inputSetting.IsPressed(inputSetting.FindAndMergeAvailableItem))
		{
			FindAndMergeAvailableItem();
		}
	}

	[EditorButton]
	[ContextMenu("MergeAllItems")]
	public void MergeAllAvailableItems()
	{
		IGrouping<int, GameItem> pair;
		while (TryGetMergeablePair(out pair))
		{
			(GameItem, GameItem) pair2 = (pair.ElementAt(0), pair.ElementAt(1));
			MergeAvailableItem(pair2);
		}
	}

	[EditorButton]
	[ContextMenu("MergePairItems")]
	public void FindAndMergeAvailableItem()
	{
		if (TryGetMergeablePair(out var pair))
		{
			(GameItem, GameItem) pair2 = (pair.ElementAt(0), pair.ElementAt(1));
			MergeAvailableItem(pair2);
		}
	}

	private bool TryGetMergeablePair(out IGrouping<int, GameItem> pair)
	{
		try
		{
			pair = (from _item in base.itemController.CurrentField.Field.Objects
				where !_item.IsHardLocked
				group _item by _item.Config.UniqId).FirstOrDefault((IGrouping<int, GameItem> _group) => _group.Count() > 1 && _group.Any((GameItem _item) => !_item.IsLocked) && base.mergeController.IsMergable(_group.First()));
		}
		catch (Exception)
		{
			pair = null;
			return false;
		}
		return pair != null;
	}

	private void MergeAvailableItem((GameItem, GameItem) pair)
	{
		if (!pair.Item1.IsLocked)
		{
			base.mergeController.Merge(pair.Item1, pair.Item2);
		}
		else
		{
			base.mergeController.Merge(pair.Item2, pair.Item1);
		}
	}
}
