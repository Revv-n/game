using System.Linq;
using GreenT.HornyScapes.MiniEvents;
using Zenject;

namespace GreenT.HornyScapes;

public sealed class RouletteDropServiceFactory : IFactory<int, RouletteLot.RewardSettings, RouletteDropService>, IFactory
{
	private readonly GarantChanceManager _garantChanceManager;

	public RouletteDropServiceFactory(GarantChanceManager garantChanceManager)
	{
		_garantChanceManager = garantChanceManager;
	}

	public RouletteDropService Create(int garantChanceId, RouletteLot.RewardSettings rewardSettings)
	{
		return new RouletteDropService(_garantChanceManager.Collection.FirstOrDefault((GarantChance gc) => gc.ID == garantChanceId), rewardSettings);
	}
}
