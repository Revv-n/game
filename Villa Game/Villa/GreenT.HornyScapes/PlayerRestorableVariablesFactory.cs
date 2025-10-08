using System;
using StripClub.Extensions;
using Zenject;

namespace GreenT.HornyScapes;

public class PlayerRestorableVariablesFactory : IFactory<int, int, int, int, string, RestorableValue<int>>, IFactory
{
	private readonly IClock _clock;

	private readonly EnergyLoadContainer _loadContainer;

	private static readonly TimeSpan oneSecond = TimeSpan.FromSeconds(1.0);

	protected PlayerRestorableVariablesFactory(IClock clock, EnergyLoadContainer loadContainer)
	{
		_clock = clock;
		_loadContainer = loadContainer;
	}

	public RestorableValue<int> Create(int initialValue, int restorePeriodInSeconds, int restoreAmountPerTick, int maxValue, string nameKey)
	{
		return new RestorableValue<int>(initialValue, _clock.GetTime, _clock.GetTime(), TimeSpan.FromSeconds(restorePeriodInSeconds), oneSecond, restoreAmountPerTick, maxValue, 0, nameKey, _loadContainer);
	}

	public RestorableEventEnergyValue<int> CreateEventEnergy(int initialValue, int restorePeriodInSeconds, int restoreAmountPerTick, int maxValue, string nameKey)
	{
		return new RestorableEventEnergyValue<int>(initialValue, _clock.GetTime, _clock.GetTime(), TimeSpan.FromSeconds(restorePeriodInSeconds), oneSecond, restoreAmountPerTick, maxValue, 0, nameKey);
	}
}
