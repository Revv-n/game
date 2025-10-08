using System.Collections;
using UnityEngine;

namespace Merge.Meta;

public class MetaInterfaceHider : InterfaceHider, IDialogActiveListener
{
	void IDialogActiveListener.AtDialogActiveChange(bool active)
	{
		StartCoroutine(CRT_SetInterfacesVisible(!active));
	}

	private IEnumerator CRT_SetInterfacesVisible(bool active)
	{
		yield return new WaitForEndOfFrame();
		SetInterfacesVisible(active);
	}
}
