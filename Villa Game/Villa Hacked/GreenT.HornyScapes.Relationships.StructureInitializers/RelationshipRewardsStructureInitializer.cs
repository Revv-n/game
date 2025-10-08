using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Data;
using GreenT.HornyScapes.Relationships.Mappers;
using GreenT.HornyScapes.Relationships.Providers;
using UniRx;

namespace GreenT.HornyScapes.Relationships.StructureInitializers;

public class RelationshipRewardsStructureInitializer : StructureInitializer<IEnumerable<RelationshipRewardMapper>>
{
	private readonly RelationshipRewardMapperProvider _manager;

	public RelationshipRewardsStructureInitializer(RelationshipRewardMapperProvider manager, IEnumerable<IStructureInitializer> others)
		: base(others)
	{
		_manager = manager;
	}

	public override IObservable<bool> Initialize(IEnumerable<RelationshipRewardMapper> mappers)
	{
		try
		{
			_manager.AddRange(mappers);
			return Observable.Do<bool>(Observable.Return(true).Debug($"{typeof(RelationshipRewardsStructureInitializer)} has been loaded", LogType.Data), (Action<bool>)delegate(bool init)
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
