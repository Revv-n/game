using System;
using System.Collections.Generic;
using UniRx;

namespace GreenT.Multiplier;

public class MultiplierDictionaryFacade<TKey, KEntity> : IMultiplierContainer<TKey> where KEntity : ICompositeMultiplier, new()
{
	public readonly IDictionary<TKey, KEntity> Dictionary;

	private readonly Subject<TKey> onAdded = new Subject<TKey>();

	public IObservable<TKey> OnAdded => onAdded.AsObservable();

	public KEntity this[TKey key]
	{
		get
		{
			if (!Dictionary.ContainsKey(key))
			{
				Dictionary[key] = new KEntity();
				onAdded?.OnNext(key);
			}
			return Dictionary[key];
		}
	}

	public MultiplierDictionaryFacade()
	{
		Dictionary = new Dictionary<TKey, KEntity>();
	}

	public void Add(TKey key, IMultiplier multiplier)
	{
		this[key].Add(multiplier);
	}

	public void Remove(TKey key, IMultiplier multiplier)
	{
		this[key].Remove(multiplier);
	}

	public IMultiplier GetMultiplier(TKey key)
	{
		return this[key];
	}

	public IEnumerable<TKey> Keys()
	{
		return Dictionary.Keys;
	}
}
