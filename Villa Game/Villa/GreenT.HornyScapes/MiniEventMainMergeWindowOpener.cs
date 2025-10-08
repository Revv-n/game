using GreenT.HornyScapes.Collection;
using GreenT.HornyScapes.MergeCore.UI;
using UnityEngine;

namespace GreenT.HornyScapes;

public class MiniEventMainMergeWindowOpener : FromTypeWindowOpener
{
	[SerializeField]
	private OpenMerge _openMerge;

	public override void Click()
	{
		_openMerge.OpenField();
		base.Click();
	}

	public override void OpenOnly()
	{
		_openMerge.OpenField();
		base.OpenOnly();
	}
}
