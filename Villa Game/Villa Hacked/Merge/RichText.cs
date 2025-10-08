using System;
using UnityEngine;

namespace Merge;

public static class RichText
{
	public static string Bold(this string text)
	{
		return $"<b>{text}</b>";
	}

	public static string Italic(this string text)
	{
		return $"<i>{text}</i>";
	}

	public static string Size(this string text, int size)
	{
		return string.Format("<size={1}>{0}</size>", text, size);
	}

	public static string Color(this string text, string hex)
	{
		return string.Format("<color=#{1}>{0}</color>", text, hex);
	}

	public static string Color(this string text, Color color)
	{
		return text.Color(ColorUtility.ToHtmlStringRGB(color));
	}

	public static char NewLine()
	{
		return '\n';
	}

	public static string CrossOut(this string text)
	{
		string text2 = string.Empty;
		for (int i = 0; i < text.Length; i++)
		{
			text2 = text2 + text[i] + "\u0336";
		}
		return text2;
	}

	public static string Superscript(this int value)
	{
		string text = string.Empty;
		string text2 = "0x207";
		string text3 = value.ToString();
		for (int i = 0; i < text3.Length; i++)
		{
			text += (char)Convert.ToInt32(text2 + text3[i], 16);
		}
		return text;
	}
}
