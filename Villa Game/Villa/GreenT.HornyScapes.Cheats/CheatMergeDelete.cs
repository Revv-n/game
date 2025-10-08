using GreenT.HornyScapes.MergeCore;
using Merge;
using UnityEngine;

namespace GreenT.HornyScapes.Cheats;

public class CheatMergeDelete : CheateMerge
{
	private void Update()
	{
		if (inputSetting.ClearCurrentField.IsPressedKeys)
		{
			ClearCurrentField();
		}
		if (inputSetting.IsPressed(inputSetting.DeleteSelectedItem))
		{
			DeleteItem();
		}
	}

	[EditorButton]
	[ContextMenu("Clear current field")]
	public void ClearCurrentField()
	{
		foreach (GameItem @object in base.itemController.CurrentField.Field.Objects)
		{
			base.itemController.RemoveItem(@object);
		}
	}

	[EditorButton]
	[ContextMenu("Delete item")]
	public void DeleteItem()
	{
		if (!(Controller<SelectionController>.Instance == null) && !(Controller<SelectionController>.Instance.Selected == null))
		{
			base.itemController.RemoveItem(Controller<SelectionController>.Instance.Selected);
		}
	}
}
