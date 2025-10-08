using UnityEngine;

namespace GreenT.HornyScapes;

public class ScrollRectSnappingModifier : ScrollRectModifier
{
	public void SnapToChild(int childId)
	{
		if (childId <= ScrollRect.content.childCount && childId >= 0)
		{
			RectTransform component = ScrollRect.content.GetChild(childId).GetComponent<RectTransform>();
			SnapToChild(component);
		}
	}

	public void SnapToChild(RectTransform target)
	{
		float value = ScrollRect.CalculateScrollToCenter(target, axis);
		if (axis == RectTransform.Axis.Vertical)
		{
			ScrollRect.verticalNormalizedPosition = Mathf.Clamp(value, 0f, 1f);
		}
		else
		{
			ScrollRect.horizontalNormalizedPosition = Mathf.Clamp(value, 0f, 1f);
		}
	}
}
