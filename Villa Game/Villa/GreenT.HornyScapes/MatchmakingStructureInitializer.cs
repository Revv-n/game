using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Data;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes;

public sealed class MatchmakingStructureInitializer : StructureInitializerViaArray<MatchmakingMapper, Matchmaking>
{
	public MatchmakingStructureInitializer(IManager<Matchmaking> manager, IFactory<MatchmakingMapper, Matchmaking> factory, IEnumerable<IStructureInitializer> others = null)
		: base(manager, factory, others)
	{
	}

	public override IObservable<bool> Initialize(IEnumerable<MatchmakingMapper> mappers)
	{
		try
		{
			foreach (MatchmakingMapper mapper in mappers)
			{
				Matchmaking matchmaking = factory.Create(mapper);
				if (matchmaking != null)
				{
					manager.Add(matchmaking);
				}
			}
			return Observable.Return(value: true).Debug(typeof(Matchmaking)?.ToString() + " has been Loaded", LogType.Data).Do(delegate(bool _init)
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
