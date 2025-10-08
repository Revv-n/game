using UnityEngine;

namespace Merge;

public static class Color32Extentions
{
	public static Color32 SetAlpha(this Color32 col, byte a)
	{
		return new Color32(col.r, col.g, col.b, a);
	}

	public static Color32 SetAlpha(this Color32 col, float a)
	{
		return new Color32(col.r, col.g, col.b, (byte)(a * 255f));
	}
}
