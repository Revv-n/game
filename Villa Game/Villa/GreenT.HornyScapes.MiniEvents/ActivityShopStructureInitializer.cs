using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Data;
using UniRx;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class ActivityShopStructureInitializer : StructureInitializer<IEnumerable<ActivityShopMapper>>
{
	private readonly IManager<ActivityShopMapper> manager;

	public ActivityShopStructureInitializer(IManager<ActivityShopMapper> manager, IEnumerable<IStructureInitializer> others = null)
		: base(others)
	{
		this.manager = manager;
	}

	public override IObservable<bool> Initialize(IEnumerable<ActivityShopMapper> mappers)
	{
		try
		{
			manager.AddRange(mappers);
			return Observable.Return(value: true).Debug(typeof(ActivityShopMapper)?.ToString() + " has been Loaded", LogType.Data).Do(delegate(bool _init)
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
