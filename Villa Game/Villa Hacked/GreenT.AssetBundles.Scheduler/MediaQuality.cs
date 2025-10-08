using System;
using System.Collections.Generic;

namespace GreenT.AssetBundles.Scheduler;

public static class MediaQuality
{
	public static Dictionary<int, QualityType> Info = new Dictionary<int, QualityType>();

	public static int GetMediaID(string url)
	{
		string[] array = url.Split('/', StringSplitOptions.None);
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == "media")
			{
				return int.Parse(array[i + 1]);
			}
		}
		return -1;
	}

	public static bool CheckSD(int id)
	{
		if (Info.TryGetValue(id, out var value))
		{
			return value == QualityType.SD;
		}
		return false;
	}
}
