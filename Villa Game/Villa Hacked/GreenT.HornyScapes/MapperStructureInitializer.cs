using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Data;
using UniRx;

namespace GreenT.HornyScapes;

public class MapperStructureInitializer<TMapper> : StructureInitializer<IEnumerable<TMapper>>
{
	public readonly IManager<TMapper> Manager;

	public MapperStructureInitializer(IManager<TMapper> manager, IEnumerable<IStructureInitializer> others = null)
		: base(others)
	{
		Manager = manager;
	}

	public override IObservable<bool> Initialize(IEnumerable<TMapper> mappers)
	{
		try
		{
			Manager.AddRange(mappers);
			return Observable.Do<bool>(Observable.Return(true), (Action<bool>)delegate(bool _init)
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
