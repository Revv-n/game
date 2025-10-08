using System.Collections.Generic;
using UnityEngine;

namespace Merge;

public class InterfaceHider : Controller<InterfaceHider>
{
	[SerializeField]
	private List<UIHiderBase> hiders;

	public void SetInterfacesVisible(bool visible)
	{
		hiders.ForEach(delegate(UIHiderBase x)
		{
			x.DoVisible(visible);
		});
	}
}
