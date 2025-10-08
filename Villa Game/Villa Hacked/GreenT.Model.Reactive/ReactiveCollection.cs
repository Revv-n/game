using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GreenT.Model.Collections;
using UniRx;

namespace GreenT.Model.Reactive;

[Serializable]
public class ReactiveCollection<T> : Collection<T>, IReactiveCollection<T>, IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable, IReadOnlyReactiveCollection<T>, IReadOnlyReactiveCollection<T>, ICollectionManager<T>, ICollectionSetter<T>, ICollectionAdder<T>, IDisposable
{
	[NonSerialized]
	private bool isDisposed;

	[NonSerialized]
	private Subject<int> countChanged;

	[NonSerialized]
	private Subject<Unit> collectionReset;

	[NonSerialized]
	private Subject<CollectionAddEvent<T>> collectionAdd;

	[NonSerialized]
	private Subject<CollectionMoveEvent<T>> collectionMove;

	[NonSerialized]
	private Subject<CollectionRemoveEvent<T>> collectionRemove;

	[NonSerialized]
	private Subject<CollectionReplaceEvent<T>> collectionReplace;

	[NonSerialized]
	private Subject<CollectionAddERangEvent<T>> collectionSet;

	private bool disposedValue;

	public ReactiveCollection()
	{
	}

	public ReactiveCollection(IEnumerable<T> collection)
	{
		if (collection == null)
		{
			throw new ArgumentNullException("collection");
		}
		foreach (T item in collection)
		{
			Add(item);
		}
	}

	public ReactiveCollection(List<T> list)
		: base((IList<T>)((list != null) ? new List<T>(list) : null))
	{
	}

	public virtual void AddItems(params T[] items)
	{
		foreach (T item in items.Distinct().Except(this))
		{
			Add(item);
		}
	}

	public virtual void RemoveItems(params T[] items)
	{
		foreach (T item in items)
		{
			Remove(item);
		}
	}

	public virtual void SetItems(params T[] items)
	{
		base.ClearItems();
		foreach (T item in items)
		{
			Add(item);
		}
		((Subject<CollectionAddERangEvent<CollectionAddERangEvent<T>>>)(object)collectionSet)?.OnNext((CollectionAddERangEvent<CollectionAddERangEvent<T>>)new CollectionAddERangEvent<T>(items));
		countChanged?.OnNext(base.Count);
	}

	protected override void ClearItems()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		int count = base.Count;
		base.ClearItems();
		if (collectionReset != null)
		{
			collectionReset.OnNext(Unit.Default);
		}
		if (count > 0 && countChanged != null)
		{
			countChanged.OnNext(base.Count);
		}
	}

	protected override void InsertItem(int index, T item)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		base.InsertItem(index, item);
		if (collectionAdd != null)
		{
			((Subject<CollectionAddEvent<CollectionAddEvent<T>>>)(object)collectionAdd).OnNext((CollectionAddEvent<CollectionAddEvent<T>>)new CollectionAddEvent<T>(index, item));
		}
		if (countChanged != null)
		{
			countChanged.OnNext(base.Count);
		}
	}

	public void Move(int oldIndex, int newIndex)
	{
		MoveItem(oldIndex, newIndex);
	}

	protected virtual void MoveItem(int oldIndex, int newIndex)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		T val = base[oldIndex];
		base.RemoveItem(oldIndex);
		base.InsertItem(newIndex, val);
		if (collectionMove != null)
		{
			((Subject<CollectionMoveEvent<CollectionMoveEvent<T>>>)(object)collectionMove).OnNext((CollectionMoveEvent<CollectionMoveEvent<T>>)new CollectionMoveEvent<T>(oldIndex, newIndex, val));
		}
	}

	protected override void RemoveItem(int index)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		T val = base[index];
		base.RemoveItem(index);
		if (collectionRemove != null)
		{
			((Subject<CollectionRemoveEvent<CollectionRemoveEvent<T>>>)(object)collectionRemove).OnNext((CollectionRemoveEvent<CollectionRemoveEvent<T>>)new CollectionRemoveEvent<T>(index, val));
		}
		if (countChanged != null)
		{
			countChanged.OnNext(base.Count);
		}
	}

	protected override void SetItem(int index, T item)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		T val = base[index];
		base.SetItem(index, item);
		if (collectionReplace != null)
		{
			((Subject<CollectionReplaceEvent<CollectionReplaceEvent<T>>>)(object)collectionReplace).OnNext((CollectionReplaceEvent<CollectionReplaceEvent<T>>)new CollectionReplaceEvent<T>(index, val, item));
		}
	}

	public IObservable<int> ObserveCountChanged(bool notifyCurrentCount = false)
	{
		if (isDisposed)
		{
			return Observable.Empty<int>();
		}
		Subject<int> val = countChanged ?? (countChanged = new Subject<int>());
		if (notifyCurrentCount)
		{
			return Observable.StartWith<int>((IObservable<int>)val, (Func<int>)(() => base.Count));
		}
		return (IObservable<int>)val;
	}

	public IObservable<Unit> ObserveReset()
	{
		if (isDisposed)
		{
			return Observable.Empty<Unit>();
		}
		return (IObservable<Unit>)(collectionReset ?? (collectionReset = new Subject<Unit>()));
	}

	public IObservable<CollectionAddEvent<T>> ObserveAdd()
	{
		if (isDisposed)
		{
			return Observable.Empty<CollectionAddEvent<T>>();
		}
		return (IObservable<CollectionAddEvent<T>>)(collectionAdd ?? (collectionAdd = (Subject<CollectionAddEvent<T>>)(object)new Subject<CollectionAddEvent<CollectionAddEvent<T>>>()));
	}

	public IObservable<CollectionMoveEvent<T>> ObserveMove()
	{
		if (isDisposed)
		{
			return Observable.Empty<CollectionMoveEvent<T>>();
		}
		return (IObservable<CollectionMoveEvent<T>>)(collectionMove ?? (collectionMove = (Subject<CollectionMoveEvent<T>>)(object)new Subject<CollectionMoveEvent<CollectionMoveEvent<T>>>()));
	}

	public IObservable<CollectionRemoveEvent<T>> ObserveRemove()
	{
		if (isDisposed)
		{
			return Observable.Empty<CollectionRemoveEvent<T>>();
		}
		return (IObservable<CollectionRemoveEvent<T>>)(collectionRemove ?? (collectionRemove = (Subject<CollectionRemoveEvent<T>>)(object)new Subject<CollectionRemoveEvent<CollectionRemoveEvent<T>>>()));
	}

	public IObservable<CollectionReplaceEvent<T>> ObserveReplace()
	{
		if (isDisposed)
		{
			return Observable.Empty<CollectionReplaceEvent<T>>();
		}
		return (IObservable<CollectionReplaceEvent<T>>)(collectionReplace ?? (collectionReplace = (Subject<CollectionReplaceEvent<T>>)(object)new Subject<CollectionReplaceEvent<CollectionReplaceEvent<T>>>()));
	}

	public IObservable<CollectionAddERangEvent<T>> ObserveSet()
	{
		if (isDisposed)
		{
			return Observable.Empty<CollectionAddERangEvent<T>>();
		}
		return (IObservable<CollectionAddERangEvent<T>>)(collectionSet ?? (collectionSet = (Subject<CollectionAddERangEvent<T>>)(object)new Subject<CollectionAddERangEvent<CollectionAddERangEvent<T>>>()));
	}

	private void DisposeSubject<TSubject>(ref Subject<TSubject> subject)
	{
		if (subject != null)
		{
			try
			{
				subject.OnCompleted();
			}
			finally
			{
				subject.Dispose();
				subject = null;
			}
		}
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
				DisposeSubject(ref collectionReset);
				DisposeSubject(ref collectionAdd);
				DisposeSubject(ref collectionMove);
				DisposeSubject(ref collectionRemove);
				DisposeSubject(ref collectionReplace);
				DisposeSubject(ref countChanged);
			}
			disposedValue = true;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
	}
}
