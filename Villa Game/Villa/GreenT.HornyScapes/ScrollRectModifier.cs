using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes;

[RequireComponent(typeof(ScrollRect), typeof(Canvas), typeof(GraphicRaycaster))]
public abstract class ScrollRectModifier : MonoBehaviour
{
	public ScrollRect ScrollRect;

	[SerializeField]
	protected RectTransform.Axis axis = RectTransform.Axis.Vertical;

	protected virtual void OnValidate()
	{
		if (ScrollRect == null)
		{
			TryGetComponent<ScrollRect>(out ScrollRect);
		}
		if (IsOneAxis())
		{
			axis = ((!ScrollRect.horizontal) ? RectTransform.Axis.Vertical : RectTransform.Axis.Horizontal);
		}
		else
		{
			Debug.LogError(GetType().Name + ": can't be used on " + base.name + ". Use only one Axis", this);
		}
	}

	private bool IsOneAxis()
	{
		if (!ScrollRect.horizontal || ScrollRect.vertical)
		{
			if (!ScrollRect.horizontal)
			{
				return ScrollRect.vertical;
			}
			return false;
		}
		return true;
	}
}
