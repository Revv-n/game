using System;
using System.Collections.Generic;
using System.Linq;

public static class Seconds
{
	public const float Minute = 60f;

	public const float Hour = 3600f;

	public const float Day = 86400f;

	public static float Get(float s = 0f, float m = 0f, float h = 0f, float d = 0f)
	{
		return s + m * 60f + h * 3600f + d * 86400f;
	}

	public static string ToLabeledString(float seconds, bool isShort = true)
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
		string text = ((timeSpan.Days > 0) ? $"{timeSpan.Days}d" : "");
		string text2 = ((timeSpan.Hours > 0) ? $"{timeSpan.Hours}h" : "");
		if (isShort && timeSpan.Days > 0)
		{
			return text + text2;
		}
		string text3 = ((timeSpan.Minutes > 0) ? $"{timeSpan.Minutes}m" : "");
		if (isShort && timeSpan.Hours > 0)
		{
			return text2 + text3;
		}
		string text4 = ((timeSpan.Seconds > 0) ? $"{timeSpan.Seconds}s" : "");
		return text + text2 + text3 + text4;
	}

	public static string ToLabeledString(float seconds, int maxEntires)
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
		List<Tuple<string, int>> list = new List<Tuple<string, int>>();
		list.Add(new Tuple<string, int>((timeSpan.Days > 0) ? $"{timeSpan.Days}d" : "", timeSpan.Days));
		list.Add(new Tuple<string, int>((timeSpan.Hours > 0) ? $"{timeSpan.Hours}h" : "", timeSpan.Hours));
		list.Add(new Tuple<string, int>((timeSpan.Minutes > 0) ? $"{timeSpan.Minutes}m" : "", timeSpan.Minutes));
		list.Add(new Tuple<string, int>((timeSpan.Seconds > 0 || !list.Any()) ? $"{timeSpan.Seconds}s" : "", timeSpan.Seconds));
		int num = maxEntires;
		bool flag = false;
		string text = "";
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].Item2 != 0 || flag)
			{
				flag = true;
				text += list[i].Item1;
				text += " ";
				num--;
				if (num == 0)
				{
					break;
				}
			}
		}
		return text;
	}
}
