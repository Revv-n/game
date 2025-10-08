using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Data;
using UniRx;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class ActivityStructureInitializer : StructureInitializer<IEnumerable<ActivityMapper>>
{
	private readonly IManager<ActivityMapper> manager;

	public ActivityStructureInitializer(IManager<ActivityMapper> manager, IEnumerable<IStructureInitializer> others = null)
		: base(others)
	{
		this.manager = manager;
	}

	public override IObservable<bool> Initialize(IEnumerable<ActivityMapper> mappers)
	{
		try
		{
			manager.AddRange(mappers);
			return Observable.Return(value: true).Debug(typeof(ActivityMapper)?.ToString() + " has been Loaded", LogType.Data).Do(delegate(bool _init)
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
