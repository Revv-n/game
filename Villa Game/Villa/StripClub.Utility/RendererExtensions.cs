using System.Linq;
using UnityEngine;

namespace StripClub.Utility;

public static class RendererExtensions
{
	private static int CountCornersVisibleFrom(this RectTransform rectTransform, Camera camera)
	{
		Rect rect = new Rect(0f, 0f, Screen.width, Screen.height);
		Vector3[] array = new Vector3[4];
		rectTransform.GetWorldCorners(array);
		int num = 0;
		for (int i = 0; i < array.Length; i++)
		{
			Vector3 point = camera.WorldToScreenPoint(array[i]);
			if (rect.Contains(point))
			{
				num++;
			}
		}
		return num;
	}

	public static bool IsFullyVisibleFrom(this RectTransform rectTransform, Camera camera)
	{
		return rectTransform.CountCornersVisibleFrom(camera) == 4;
	}

	public static bool IsVisibleFrom(this RectTransform rectTransform, Camera camera)
	{
		return rectTransform.CountCornersVisibleFrom(camera) > 0;
	}

	public static bool Overlaps(this RectTransform rectTransform, RectTransform another)
	{
		Rect rect = new Rect(rectTransform.position.x, rectTransform.position.y, rectTransform.rect.width, rectTransform.rect.height);
		Rect other = new Rect(another.position.x, another.position.y, another.rect.width, another.rect.height);
		return rect.Overlaps(other);
	}

	public static bool IsFullyNested(this RectTransform rectTransform, RectTransform target)
	{
		Vector3[] array = new Vector3[4];
		rectTransform.GetWorldCorners(array);
		return array.All((Vector3 _corner) => target.rect.Contains(target.InverseTransformPoint(_corner)));
	}
}
