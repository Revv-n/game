using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Data;
using GreenT.HornyScapes.Events;
using UniRx;

namespace GreenT.HornyScapes;

public sealed class MiniEventStructureInitializer : StructureInitializer<IEnumerable<MiniEventMapper>>
{
	private readonly IManager<MiniEventMapper> manager;

	public MiniEventStructureInitializer(IManager<MiniEventMapper> manager, IEnumerable<IStructureInitializer> others = null)
		: base(others)
	{
		this.manager = manager;
	}

	public override IObservable<bool> Initialize(IEnumerable<MiniEventMapper> mappers)
	{
		try
		{
			manager.AddRange(mappers);
			return Observable.Do<bool>(Observable.Return(true).Debug(typeof(MiniEventMapper)?.ToString() + " has been Loaded", LogType.Data), (Action<bool>)delegate(bool _init)
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
