using UnityEngine;

namespace Merge;

public static class RectTransformExtention
{
	public static void SetSizeY(this RectTransform rt, float value)
	{
		rt.sizeDelta = new Vector2(rt.sizeDelta.x, value);
	}

	public static void SetSizeX(this RectTransform rt, float value)
	{
		rt.sizeDelta = new Vector2(value, rt.sizeDelta.y);
	}

	public static void AddSizeY(this RectTransform rt, float value)
	{
		rt.sizeDelta = new Vector2(rt.sizeDelta.x, rt.sizeDelta.y + value);
	}

	public static void AddSizeX(this RectTransform rt, float value)
	{
		rt.sizeDelta = new Vector2(rt.sizeDelta.x + value, rt.sizeDelta.y);
	}

	public static void SetAnchoerdPosY(this RectTransform rt, float value)
	{
		rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, value);
	}

	public static void SetAnchoerdPosX(this RectTransform rt, float value)
	{
		rt.anchoredPosition = new Vector2(value, rt.anchoredPosition.y);
	}

	public static Rect GetWorldRect(this RectTransform rt)
	{
		Vector3[] array = new Vector3[4];
		rt.GetWorldCorners(array);
		Vector3 vector = new Vector3(array[0].x, array[0].y, 0f);
		return new Rect(size: new Vector3(array[2].x - array[0].x, array[2].y - array[0].y, 0f), position: vector);
	}
}
