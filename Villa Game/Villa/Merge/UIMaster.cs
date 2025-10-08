using System.Collections;
using UnityEngine;

namespace Merge;

public class UIMaster : Controller<UIMaster>
{
	[SerializeField]
	private Canvas generalUiCanvas;

	[SerializeField]
	private Canvas overlayUiCanvas;

	[SerializeField]
	private Camera mainCamera;

	[SerializeField]
	private InterfaceHider interfaceHider;

	[SerializeField]
	private GameObject[] objectsToHide;

	public static Canvas GeneralUiCanvas => Controller<UIMaster>.Instance.generalUiCanvas;

	public static Canvas OverlayUiCanvas => Controller<UIMaster>.Instance.overlayUiCanvas;

	public static Camera MainCamera => Controller<UIMaster>.Instance.mainCamera;

	public static InterfaceHider InterfaceHider => Controller<UIMaster>.Instance.interfaceHider;

	public IEnumerator HideObjects()
	{
		yield return new WaitForEndOfFrame();
		if (objectsToHide != null && objectsToHide.Length != 0)
		{
			GameObject[] array = objectsToHide;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: false);
			}
		}
	}
}
