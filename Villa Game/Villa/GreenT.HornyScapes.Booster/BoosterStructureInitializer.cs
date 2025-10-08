using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Data;
using UniRx;

namespace GreenT.HornyScapes.Booster;

public class BoosterStructureInitializer : StructureInitializer<IEnumerable<BoosterMapper>>
{
	private readonly IManager<BoosterMapper> _manager;

	public BoosterStructureInitializer(IManager<BoosterMapper> manager, IEnumerable<IStructureInitializer> others = null)
		: base(others)
	{
		_manager = manager;
	}

	public override IObservable<bool> Initialize(IEnumerable<BoosterMapper> param)
	{
		try
		{
			_manager.AddRange(param);
			return Observable.Return(value: true).Do(delegate(bool _init)
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
