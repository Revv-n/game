using System;
using System.Collections.Generic;
using GreenT;
using UniRx;
using Zenject;

namespace StripClub.Model.Data;

public class EntityLoader<TMapper, KEntity> : ILoader<IEnumerable<KEntity>>
{
	private readonly ILoader<IEnumerable<TMapper>> loader;

	private readonly IFactory<TMapper, KEntity> factory;

	public EntityLoader(ILoader<IEnumerable<TMapper>> loader, IFactory<TMapper, KEntity> factory)
	{
		this.loader = loader;
		this.factory = factory;
	}

	public IObservable<IEnumerable<KEntity>> Load()
	{
		return Observable.ToArray<KEntity>(Observable.Catch<KEntity, Exception>(Observable.Select<TMapper, KEntity>(Observable.SelectMany<IEnumerable<TMapper>, TMapper>(loader.Load(), (Func<IEnumerable<TMapper>, IEnumerable<TMapper>>)((IEnumerable<TMapper> x) => x)), (Func<TMapper, KEntity>)factory.Create), (Func<Exception, IObservable<KEntity>>)delegate(Exception ex)
		{
			throw ex.SendException(GetType().Name + ": Error when creating " + typeof(KEntity).Name + " by factory: " + ((object)factory).GetType().Name + "\n");
		}));
	}
}
