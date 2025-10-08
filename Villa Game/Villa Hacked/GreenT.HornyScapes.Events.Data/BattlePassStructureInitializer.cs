using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Data;
using UniRx;

namespace GreenT.HornyScapes.Events.Data;

public class BattlePassStructureInitializer : StructureInitializer<IEnumerable<BattlePassMapper>>
{
	private readonly IManager<BattlePassMapper> manager;

	public BattlePassStructureInitializer(IManager<BattlePassMapper> manager, IEnumerable<IStructureInitializer> others = null)
		: base(others)
	{
		this.manager = manager;
	}

	public override IObservable<bool> Initialize(IEnumerable<BattlePassMapper> mappers)
	{
		try
		{
			manager.AddRange(mappers);
			return Observable.Do<bool>(Observable.Return(true).Debug(typeof(BattlePassMapper)?.ToString() + " has been Loaded", LogType.Data), (Action<bool>)delegate(bool _init)
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
