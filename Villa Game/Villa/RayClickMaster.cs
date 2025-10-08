using System.Collections.Generic;
using System.Linq;
using Merge.Core.Masters;
using UnityEngine;

public class RayClickMaster : Controller<RayClickMaster>, IClickController
{
	[SerializeField]
	private Camera camera;

	void IClickController.AtClick(Vector3 position, Vector3 downPosition)
	{
		RaycastHit[] array = Physics.RaycastAll(camera.ScreenPointToRay(Input.mousePosition));
		List<IRayClickable> list = new List<IRayClickable>();
		RaycastHit[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			RaycastHit raycastHit = array2[i];
			if (!(raycastHit.collider == null))
			{
				IRayClickable component = raycastHit.collider.GetComponent<IRayClickable>();
				if (component != null && component.IsRayClickEnable)
				{
					list.Add(component);
				}
			}
		}
		if (list.Count != 0)
		{
			list.OrderByDescending((IRayClickable x) => x.RayClickOrder).First().AtRayClick();
		}
	}
}
