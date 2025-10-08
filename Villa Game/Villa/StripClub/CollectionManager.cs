using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;

namespace StripClub;

public class CollectionManager<T> : IDisposable
{
	protected List<T> collection = new List<T>();

	private Subject<T> onNew = new Subject<T>();

	public IEnumerable<T> Collection => collection.AsEnumerable();

	public IObservable<T> OnNew => onNew;

	public virtual void Add(T element)
	{
		collection.Add(element);
		onNew.OnNext(element);
	}

	public virtual void AddRange(IEnumerable<T> elements)
	{
		collection.AddRange(elements);
		foreach (T element in elements)
		{
			onNew.OnNext(element);
		}
	}

	public void Dispose()
	{
		onNew?.Dispose();
	}
}
