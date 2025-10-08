using System;
using UniRx;

namespace GreenT.Model.Collections;

public abstract class Storage<TEntity> : SimpleManager<TEntity>
{
	protected readonly Subject<TEntity> onRemove = new Subject<TEntity>();

	public virtual IObservable<TEntity> OnRemove => Observable.AsObservable<TEntity>((IObservable<TEntity>)onRemove);

	public virtual void Remove(TEntity entity)
	{
		if (collection.Contains(entity))
		{
			collection.Remove(entity);
			onRemove.OnNext(entity);
		}
	}
}
