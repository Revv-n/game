using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Data;
using UniRx;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class ActivityQuestStructureInitializer : StructureInitializer<IEnumerable<ActivityQuestMapper>>
{
	private readonly IManager<ActivityQuestMapper> manager;

	public ActivityQuestStructureInitializer(IManager<ActivityQuestMapper> manager, IEnumerable<IStructureInitializer> others = null)
		: base(others)
	{
		this.manager = manager;
	}

	public override IObservable<bool> Initialize(IEnumerable<ActivityQuestMapper> mappers)
	{
		try
		{
			manager.AddRange(mappers);
			return Observable.Return(value: true).Debug(typeof(ActivityQuestMapper)?.ToString() + " has been Loaded", LogType.Data).Do(delegate(bool _init)
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
