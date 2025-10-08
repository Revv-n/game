using System;
using System.Collections.Generic;
using GreenT;
using UniRx;
using Zenject;

namespace StripClub.Model.Data;

public class EntityLoaderWithId<TMapper, KEntity> : ILoader<IEnumerable<KEntity>>
{
	private readonly ILoader<IEnumerable<TMapper>> loader;

	private readonly IFactory<TMapper, KEntity> factory;

	public EntityLoaderWithId(ILoader<IEnumerable<TMapper>> loader, IFactory<TMapper, KEntity> factory)
	{
		this.loader = loader;
		this.factory = factory;
	}

	public IObservable<IEnumerable<KEntity>> Load()
	{
		return loader.Load().SelectMany((IEnumerable<TMapper> x) => x).Select(factory.Create)
			.Catch(delegate(Exception ex)
			{
				throw ex.SendException(GetType().Name + ": Error when creating " + typeof(KEntity).Name + " by factory: " + factory.GetType().Name + "\n");
			})
			.ToArray();
	}
}
