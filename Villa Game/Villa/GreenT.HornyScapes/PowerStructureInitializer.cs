using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Data;
using UniRx;

namespace GreenT.HornyScapes;

public class PowerStructureInitializer : StructureInitializer<IEnumerable<PowerMapper>>
{
	private readonly IManager<PowerMapper> manager;

	public PowerStructureInitializer(IManager<PowerMapper> manager, IEnumerable<IStructureInitializer> others = null)
		: base(others)
	{
		this.manager = manager;
	}

	public override IObservable<bool> Initialize(IEnumerable<PowerMapper> mappers)
	{
		try
		{
			manager.AddRange(mappers);
			return Observable.Return(value: true).Debug(typeof(PowerMapper)?.ToString() + " has been Loaded", LogType.Data).Do(delegate(bool _init)
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
