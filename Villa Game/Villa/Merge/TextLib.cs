using System;
using UnityEngine;

namespace Merge;

public static class TextLib
{
	public static readonly string[] BIG_KEYS = new string[55]
	{
		"k", "B", "T", "aa", "ab", "ac", "ad", "ae", "af", "ag",
		"ah", "ai", "aj", "ak", "al", "am", "an", "ao", "ap", "aq",
		"ar", "as", "at", "au", "av", "aw", "ax", "ay", "az", "ba",
		"bb", "bc", "bd", "be", "bf", "bg", "bh", "bi", "bj", "bk",
		"bl", "bm", "bn", "bo", "bp", "bq", "br", "bs", "bt", "bu",
		"bv", "bw", "bx", "by", "bz"
	};

	public static string ToBigValue(double value)
	{
		if (value < 1000.0)
		{
			return SmartRound(value).ToString();
		}
		for (int i = 0; i < BIG_KEYS.Length; i++)
		{
			double num = Math.Pow(10.0, (i + 2) * 3);
			if (value < num)
			{
				return $"{SmartRound(value / num * 1000.0)}{BIG_KEYS[i]}";
			}
		}
		throw new Exception();
	}

	private static double SmartRound(double value)
	{
		if (value < 1.0)
		{
			return Math.Round(value, 3);
		}
		if (value < 10.0)
		{
			return Math.Round(value, 2);
		}
		if (value < 100.0)
		{
			return Math.Round(value, 1);
		}
		return Math.Round(value);
	}

	public static string ToHoursTime(float seconds, bool short_format = true)
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
		string obj = ((timeSpan.Hours > 0) ? $"{timeSpan.Hours}h" : "");
		string text = ((timeSpan.Minutes > 0) ? $"{timeSpan.Minutes}m" : "");
		string text2 = (((timeSpan.Hours > 0 && short_format) || (float)timeSpan.Seconds < Mathf.Epsilon) ? "" : $"{timeSpan.Seconds}s");
		return obj + text + text2;
	}

	public static string ToDaysTime(float seconds, bool short_format = true)
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
		string text = ((timeSpan.Days > 0) ? $"{timeSpan.Days}d" : "");
		string text2 = ((timeSpan.Hours > 0) ? $"{timeSpan.Hours}h" : "");
		if (short_format && timeSpan.Days > 0)
		{
			return text + text2;
		}
		string text3 = ((timeSpan.Minutes > 0) ? $"{timeSpan.Minutes}m" : "");
		if (short_format && timeSpan.Hours > 0)
		{
			return text2 + text3;
		}
		string text4 = ((timeSpan.Seconds > 0) ? $"{timeSpan.Seconds}s" : "");
		return text + text2 + text3 + text4;
	}

	public static string ToShortTime(float t, bool isNeedMilli = false)
	{
		if (t < 0f)
		{
			return "";
		}
		TimeSpan timeSpan = TimeSpan.FromSeconds(t);
		if (!isNeedMilli)
		{
			return $"{timeSpan.Seconds}";
		}
		return $"{timeSpan.Seconds}.{timeSpan.Milliseconds / 100}";
	}

	public static string ColorizeBool(bool value)
	{
		return value.ToString().ToUpper().SetFontColor(value ? StringColors.green : StringColors.red);
	}
}
