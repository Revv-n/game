using UnityEngine;
using UnityEngine.UI;

namespace Merge;

public static class ImageExtention
{
	public static void SetAlpha(this Image img, float a)
	{
		Color color = img.color;
		color.a = a;
		img.color = color;
	}

	public static float GetAlpha(this Image img)
	{
		return img.color.a;
	}
}
