using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;

namespace GreenT.Model.Collections;

public abstract class SimpleManager<TEntity> : IManager<TEntity>, IDisposable
{
	protected readonly List<TEntity> collection = new List<TEntity>();

	protected readonly Subject<TEntity> onNew = new Subject<TEntity>();

	public virtual IEnumerable<TEntity> Collection => collection.AsEnumerable();

	public virtual IObservable<TEntity> OnNew => Observable.AsObservable<TEntity>((IObservable<TEntity>)onNew);

	public virtual void Add(TEntity entity)
	{
		collection.Add(entity);
		onNew.OnNext(entity);
	}

	public virtual void AddRange(IEnumerable<TEntity> entities)
	{
		collection.AddRange(entities);
		foreach (TEntity entity in entities)
		{
			onNew.OnNext(entity);
		}
	}

	public virtual void Dispose()
	{
		onNew.Dispose();
	}
}
