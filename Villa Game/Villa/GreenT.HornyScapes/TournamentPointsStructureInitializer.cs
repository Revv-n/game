using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Data;
using UniRx;

namespace GreenT.HornyScapes;

public sealed class TournamentPointsStructureInitializer : StructureInitializer<IEnumerable<TournamentPointsMapper>>
{
	private readonly IManager<TournamentPointsMapper> manager;

	public TournamentPointsStructureInitializer(IManager<TournamentPointsMapper> manager, IEnumerable<IStructureInitializer> others = null)
		: base(others)
	{
		this.manager = manager;
	}

	public override IObservable<bool> Initialize(IEnumerable<TournamentPointsMapper> mappers)
	{
		try
		{
			manager.AddRange(mappers);
			return Observable.Return(value: true).Debug(typeof(TournamentPointsMapper)?.ToString() + " has been Loaded", LogType.Data).Do(delegate(bool _init)
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
