using System.Collections.Generic;
using UnityEngine;

namespace Merge;

public static class IconProvider
{
	private const string GI_ROOT = "GIIcons";

	private static Dictionary<string, Sprite> cachedDict = new Dictionary<string, Sprite>();

	public static Sprite GetSprite(string path)
	{
		if (cachedDict.TryGetValue(path, out var value))
		{
			return value;
		}
		value = Resources.Load<Sprite>(path);
		cachedDict.Add(path, value);
		return value;
	}

	public static Sprite GetGISprite(GIKey key)
	{
		return GetSprite(string.Format("{0}/{1}/{2}{3}", "GIIcons", key.Collection, key.Collection, key.ID));
	}
}
