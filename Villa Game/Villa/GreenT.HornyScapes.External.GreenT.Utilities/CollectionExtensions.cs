using System;
using System.Collections.Generic;
using UniRx;

namespace GreenT.HornyScapes.External.GreenT.Utilities;

public static class CollectionExtensions
{
	public static IObservable<TValue> OnValueChange<T, TValue>(this Dictionary<T, TValue> dictionary, T key)
	{
		return Observable.EveryUpdate().ObserveOn(Scheduler.MainThreadEndOfFrame).ObserveEveryValueChanged((IObservable<long> _) => dictionary[key]);
	}

	public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
	{
		HashSet<TKey> knownKeys = new HashSet<TKey>();
		foreach (TSource item in source)
		{
			if (knownKeys.Add(keySelector(item)))
			{
				yield return item;
			}
		}
	}
}
