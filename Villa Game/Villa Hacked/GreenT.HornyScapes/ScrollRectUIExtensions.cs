using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes;

public static class ScrollRectUIExtensions
{
	private static Vector3[] corners = new Vector3[4];

	public static float CalculateScrollToCenter(this ScrollRect scrollRect, RectTransform target, RectTransform.Axis axis = RectTransform.Axis.Vertical)
	{
		RectTransform rectTransform = scrollRect.viewport ?? scrollRect.GetComponent<RectTransform>();
		Rect rect = rectTransform.rect;
		Bounds bounds = target.TransformBoundsTo(rectTransform);
		if (axis == RectTransform.Axis.Vertical)
		{
			float distance = rect.center.y - bounds.center.y;
			return scrollRect.verticalNormalizedPosition - scrollRect.NormalizeScrollDistance(1, distance);
		}
		float distance2 = rect.center.x - bounds.center.x;
		return scrollRect.horizontalNormalizedPosition - scrollRect.NormalizeScrollDistance(0, distance2);
	}

	private static Bounds TransformBoundsTo(this RectTransform source, Transform target)
	{
		Bounds result = default(Bounds);
		if (source != null)
		{
			source.GetWorldCorners(corners);
			Vector3 vector = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
			Vector3 vector2 = new Vector3(float.MinValue, float.MinValue, float.MinValue);
			Matrix4x4 worldToLocalMatrix = target.worldToLocalMatrix;
			for (int i = 0; i < 4; i++)
			{
				Vector3 lhs = worldToLocalMatrix.MultiplyPoint3x4(corners[i]);
				vector = Vector3.Min(lhs, vector);
				vector2 = Vector3.Max(lhs, vector2);
			}
			result = new Bounds(vector, Vector3.zero);
			result.Encapsulate(vector2);
		}
		return result;
	}

	private static float NormalizeScrollDistance(this ScrollRect scrollRect, int axis, float distance)
	{
		RectTransform viewport = scrollRect.viewport;
		RectTransform rectTransform = ((viewport != null) ? viewport : scrollRect.GetComponent<RectTransform>());
		Bounds bounds = new Bounds(rectTransform.rect.center, rectTransform.rect.size);
		RectTransform content = scrollRect.content;
		float num = ((content != null) ? content.TransformBoundsTo(rectTransform) : default(Bounds)).size[axis] - bounds.size[axis];
		return distance / num;
	}
}
