using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Data;
using GreenT.HornyScapes.Sellouts.Mappers;
using GreenT.HornyScapes.Sellouts.Providers;
using UniRx;

namespace GreenT.HornyScapes.Sellouts.StructureInitializers;

public class SelloutStructureInitializer : StructureInitializer<IEnumerable<SelloutMapper>>
{
	private readonly SelloutMapperProvider _mapperProvider;

	public SelloutStructureInitializer(SelloutMapperProvider mapperProvider, IEnumerable<IStructureInitializer> others = null)
		: base(others)
	{
		_mapperProvider = mapperProvider;
	}

	public override IObservable<bool> Initialize(IEnumerable<SelloutMapper> mappers)
	{
		try
		{
			_mapperProvider.AddRange(mappers);
			return Observable.Return(value: true).Debug(typeof(SelloutMapper)?.ToString() + " has been Loaded", LogType.Data).Do(delegate(bool isInited)
			{
				isInitialized.Value = isInited;
			});
		}
		catch (Exception exception)
		{
			throw exception.LogException();
		}
	}
}
