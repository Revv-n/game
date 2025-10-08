using System;
using System.Collections.Generic;
using StripClub.Model.Data;
using UniRx;

namespace GreenT.HornyScapes;

public class ConfigDataLoader<TEntity> : ILoader<IEnumerable<TEntity>>
{
	private readonly MapperStructureInitializer<TEntity> mapperStructure;

	public ConfigDataLoader(MapperStructureInitializer<TEntity> mapperStructure)
	{
		this.mapperStructure = mapperStructure;
	}

	public IObservable<IEnumerable<TEntity>> Load()
	{
		if (!mapperStructure.IsInitialized.Value)
		{
			throw new Exception(GetType().Name + ": You must initialize structure before (" + mapperStructure.GetType().Name + ")");
		}
		return Observable.Return<IEnumerable<TEntity>>(mapperStructure.Manager.Collection, Scheduler.MainThreadIgnoreTimeScale);
	}
}
