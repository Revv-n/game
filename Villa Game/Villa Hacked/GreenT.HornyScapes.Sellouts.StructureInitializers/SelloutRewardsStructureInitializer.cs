using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Data;
using GreenT.HornyScapes.Sellouts.Mappers;
using GreenT.HornyScapes.Sellouts.Providers;
using UniRx;

namespace GreenT.HornyScapes.Sellouts.StructureInitializers;

public class SelloutRewardsStructureInitializer : StructureInitializer<IEnumerable<SelloutRewardsMapper>>
{
	private readonly SelloutRewardsMapperProvider _manager;

	public SelloutRewardsStructureInitializer(SelloutRewardsMapperProvider manager, IEnumerable<IStructureInitializer> others)
		: base(others)
	{
		_manager = manager;
	}

	public override IObservable<bool> Initialize(IEnumerable<SelloutRewardsMapper> mappers)
	{
		try
		{
			_manager.AddRange(mappers);
			return Observable.Do<bool>(Observable.Return(true).Debug($"{typeof(SelloutRewardsStructureInitializer)} has been loaded", LogType.Data), (Action<bool>)delegate(bool init)
			{
				isInitialized.Value = init;
			});
		}
		catch (Exception exception)
		{
			throw exception.LogException();
		}
	}
}
