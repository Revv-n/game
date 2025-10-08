using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UniRx;
using UnityEngine;

namespace Hedge.Tools;

public static class FormatGameText
{
	public enum NumberFormat
	{
		ShortSacleUS,
		Exponential,
		MetricScale
	}

	private static readonly List<string> shortScaleUS;

	private static ReactiveProperty<NumberFormat> numberFormat;

	public static IReadOnlyReactiveProperty<NumberFormat> NumberFormatStream => (IReadOnlyReactiveProperty<NumberFormat>)(object)ReactivePropertyExtensions.ToReadOnlyReactiveProperty<NumberFormat>((IObservable<NumberFormat>)numberFormat);

	public static NumberFormat Format
	{
		get
		{
			return numberFormat.Value;
		}
		set
		{
			numberFormat.Value = value;
		}
	}

	public static IObservable<string> ToObservableShortNumber<T>(this T value)
	{
		return Observable.Select<NumberFormat, string>((IObservable<NumberFormat>)NumberFormatStream, (Func<NumberFormat, string>)((NumberFormat x) => value.ToShortNumber()));
	}

	static FormatGameText()
	{
		numberFormat = new ReactiveProperty<NumberFormat>(NumberFormat.Exponential);
		string[] array = new string[9] { "d", "vg", "tg", "qg", "Qg", "sg", "Sg", "Og", "Ng" };
		shortScaleUS = new List<string>();
		shortScaleUS.Add("");
		shortScaleUS.Add("K");
		shortScaleUS.Add("M");
		shortScaleUS.Add("B");
		shortScaleUS.Add("T");
		shortScaleUS.Add("Qa");
		shortScaleUS.Add("Qi");
		shortScaleUS.Add("Sx");
		shortScaleUS.Add("Sp");
		shortScaleUS.Add("Oc");
		shortScaleUS.Add("No");
		for (int i = 0; i < array.Length; i++)
		{
			shortScaleUS.Add(array[i]);
			shortScaleUS.Add("U" + array[i]);
			shortScaleUS.Add("D" + array[i]);
			shortScaleUS.Add("Tr" + array[i]);
			shortScaleUS.Add("Qa" + array[i]);
			shortScaleUS.Add("Qi" + array[i]);
			shortScaleUS.Add("Sx" + array[i]);
			shortScaleUS.Add("Sp" + array[i]);
			shortScaleUS.Add("Oc" + array[i]);
			shortScaleUS.Add("No" + array[i]);
		}
	}

	public static string ToShortNumber<T>(this T value)
	{
		string text = string.Format(CultureInfo.InvariantCulture, "{0:#;#;0}", value);
		int num = (text.Length - 1) / 3;
		int num2 = (text.Length - 1) % 3 + 1;
		int num3 = Mathf.Clamp(text.Length - num2, 0, 2);
		try
		{
			float num4 = float.Parse(text.Substring(0, num2));
			if (num3 > 0)
			{
				num4 += float.Parse(text.Substring(num2, num3)) / 100f;
			}
			StringBuilder stringBuilder = new StringBuilder();
			if (value.ToString()[0] == '-')
			{
				stringBuilder.Append('-');
			}
			if (num > 0)
			{
				stringBuilder.Append("{0:#.00}");
			}
			else
			{
				stringBuilder.Append("{0:0}");
			}
			stringBuilder.Append("{1}");
			string arg = "";
			if (num > 0)
			{
				switch (Format)
				{
				case NumberFormat.ShortSacleUS:
					arg = " " + shortScaleUS[num];
					break;
				case NumberFormat.Exponential:
					arg = " e" + num * 3;
					break;
				default:
					Debug.LogError("There is no scenario for this format");
					break;
				}
			}
			return string.Format(CultureInfo.InvariantCulture, stringBuilder.ToString(), num4, arg);
		}
		catch
		{
			Debug.LogWarning("this method can't make non-numerical types shorter :)");
			return value.ToString();
		}
	}
}
