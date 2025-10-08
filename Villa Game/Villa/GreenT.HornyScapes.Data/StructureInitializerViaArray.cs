using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Data;

public class StructureInitializerViaArray<TMapper, TEntity> : StructureInitializer<IEnumerable<TMapper>>
{
	protected readonly IManager<TEntity> manager;

	protected readonly IFactory<TMapper, TEntity> factory;

	public StructureInitializerViaArray(IManager<TEntity> manager, IFactory<TMapper, TEntity> factory, IEnumerable<IStructureInitializer> others = null)
		: base(others)
	{
		this.manager = manager;
		this.factory = factory;
	}

	public override IObservable<bool> Initialize(IEnumerable<TMapper> mappers)
	{
		try
		{
			foreach (TMapper mapper in mappers)
			{
				TEntity val = factory.Create(mapper);
				if (val != null)
				{
					manager.Add(val);
				}
			}
			return Observable.Return(value: true).Debug(typeof(TEntity)?.ToString() + " has been Loaded", LogType.Data).Do(delegate(bool _init)
			{
				isInitialized.Value = _init;
			});
		}
		catch (Exception exception)
		{
			throw exception.LogException();
		}
	}
}
public class StructureInitializerViaArray<TMapper, TEntity, TEntityBase> : StructureInitializer<IEnumerable<TMapper>> where TEntity : TEntityBase
{
	public readonly IManager<TEntityBase> Manager;

	public readonly IFactory<TMapper, TEntity> Factory;

	public StructureInitializerViaArray(IManager<TEntityBase> manager, IFactory<TMapper, TEntity> factory, IEnumerable<IStructureInitializer> others = null)
		: base(others)
	{
		Manager = manager;
		Factory = factory;
	}

	public override IObservable<bool> Initialize(IEnumerable<TMapper> mappers)
	{
		try
		{
			TEntityBase[] entities = mappers.Select(Factory.Create).OfType<TEntityBase>().ToArray();
			Manager.AddRange(entities);
			return Observable.Return(value: true).Debug(typeof(TEntity)?.ToString() + " has been Loaded", LogType.Data).Do(delegate(bool _init)
			{
				isInitialized.Value = _init;
			});
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}
}
