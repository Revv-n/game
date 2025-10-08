using UnityEngine;
using UnityEngine.UI;

namespace Merge;

public static class TextExtention
{
	public static void SetAlpha(this Text img, float a)
	{
		Color color = img.color;
		color.a = a;
		img.color = color;
	}

	public static float GetAlpha(this Text img)
	{
		return img.color.a;
	}
}
