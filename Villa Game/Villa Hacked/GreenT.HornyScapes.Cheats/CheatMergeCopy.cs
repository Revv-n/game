using GreenT.HornyScapes.MergeCore;
using UnityEngine;

namespace GreenT.HornyScapes.Cheats;

public class CheatMergeCopy : CheateMerge
{
	private void Update()
	{
		if (inputSetting.CopyMergeItemId.IsPressedKeys)
		{
			CopyMergeItemId();
		}
	}

	[EditorButton]
	[ContextMenu("Copy Merge Item Id")]
	public void CopyMergeItemId()
	{
		if (!(Controller<SelectionController>.Instance == null) && !(Controller<SelectionController>.Instance.Selected == null))
		{
			CopyUtil.Copy(Controller<SelectionController>.Instance.Selected.Config.UniqId.ToString());
		}
	}
}
