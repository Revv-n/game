using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace StripClub.Extensions;

public static class ExtensionMethods
{
	public static Random random = new Random();

	public static T Random<T>(this IEnumerable<T> list)
	{
		return list.ElementAt(random.Next(list.Count()));
	}

	public static T Random<T>(this T[] array)
	{
		return array[random.Next(array.Length)];
	}

	public static IEnumerable<T> Random<T>(this IEnumerable<T> collection, int countOfRandomElements)
	{
		T[] array = collection.ToArray();
		int collectionSize = array.Length;
		for (int id = 0; id < countOfRandomElements; id++)
		{
			yield return array[random.Next(collectionSize)];
		}
	}

	public static IEnumerable<T> AsEnumerable<T>(this T item)
	{
		yield return item;
	}

	public static T Multiply<T, K>(T a, K b) where K : struct, IComparable, IComparable<K>, IEquatable<K>
	{
		return GenericArithmetic<T, K>.Multiply(a, b);
	}

	public static K Add<K>(K a, K b)
	{
		return GenericArithmetic<K, K>.Add(a, b);
	}

	public static K Substract<K>(K a, K b)
	{
		return GenericArithmetic<K, K>.Subtract(a, b);
	}

	public static double Clamp(double value, double min, double max)
	{
		return Math.Min(Math.Max(value, min), max);
	}

	public static string FirstCharToUpper(this string input)
	{
		if (input != null)
		{
			if (input == "")
			{
				throw new ArgumentException("input cannot be empty", "input");
			}
			return input.First().ToString().ToUpper() + input.Substring(1);
		}
		throw new ArgumentNullException("input");
	}

	public static string PathCombineUnixStyle(params string[] path)
	{
		return Path.Combine(path).Replace('\\', '/');
	}
}
