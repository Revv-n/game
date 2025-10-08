using System;
using System.Collections.Generic;

namespace GreenT.HornyScapes.Extensions;

public static class DictionaryExtension
{
	public static void AddRange<K, T>(this IDictionary<K, T> target, IDictionary<K, T> source, bool set = true)
	{
		foreach (KeyValuePair<K, T> item in source)
		{
			K key = item.Key;
			T value = item.Value;
			if (set)
			{
				target[key] = value;
			}
			else
			{
				target.Add(key, value);
			}
		}
	}

	public static void AddRange<K, T, TI>(this IDictionary<K, T> target, IEnumerable<TI> source, Func<TI, K> key, Func<TI, T> selector, bool set = true)
	{
		foreach (TI item in source)
		{
			K key2 = key(item);
			T value = selector(item);
			if (set)
			{
				target[key2] = value;
			}
			else
			{
				target.Add(key(item), selector(item));
			}
		}
	}
}
