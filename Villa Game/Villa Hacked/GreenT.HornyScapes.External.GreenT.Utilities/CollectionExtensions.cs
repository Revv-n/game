using System;
using System.Collections.Generic;
using UniRx;

namespace GreenT.HornyScapes.External.GreenT.Utilities;

public static class CollectionExtensions
{
	public static IObservable<TValue> OnValueChange<T, TValue>(this Dictionary<T, TValue> dictionary, T key)
	{
		return ObserveExtensions.ObserveEveryValueChanged<IObservable<long>, TValue>(Observable.ObserveOn<long>(Observable.EveryUpdate(), Scheduler.MainThreadEndOfFrame), (Func<IObservable<long>, TValue>)((IObservable<long> _) => dictionary[key]), (FrameCountType)0, false);
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
