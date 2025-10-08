using System;
using GreenT.Bonus;

namespace GreenT.Multiplier;

public class MultiplierManager
{
	public readonly AdjustedMultiplierDictionary<int, SummingCompositeMultiplier> SpawnerMaxAmountMultipliers;

	public readonly AdjustedMultiplierDictionary<int, SummingCompositeMultiplier> SpawnerProductionMultipliers;

	public readonly AdjustedMultiplierDictionary<int, MultiplyingCompositeMultiplier> SpawnerReloadMultipliers;

	public readonly AdjustedMultiplierDictionary<int, BoosterSummingMultiplier> IncreaseEnergyMultipliers;

	public readonly AdjustedMultiplierDictionary<int, BoosterSummingMultiplier> IncreaseEnergyRechargeSpeedMultipliers;

	public readonly AdjustedMultiplierDictionary<int, MinCompositeMultiplier> SummonMultipliers;

	public MultiplierManager()
	{
		SpawnerMaxAmountMultipliers = new AdjustedMultiplierDictionary<int, SummingCompositeMultiplier>();
		SpawnerProductionMultipliers = new AdjustedMultiplierDictionary<int, SummingCompositeMultiplier>();
		SpawnerReloadMultipliers = new AdjustedMultiplierDictionary<int, MultiplyingCompositeMultiplier>();
		IncreaseEnergyMultipliers = new AdjustedMultiplierDictionary<int, BoosterSummingMultiplier>();
		IncreaseEnergyRechargeSpeedMultipliers = new AdjustedMultiplierDictionary<int, BoosterSummingMultiplier>();
		SummonMultipliers = new AdjustedMultiplierDictionary<int, MinCompositeMultiplier>();
	}

	public IMultiplierTotalContainer<int> GetCollection(BonusType bonusType)
	{
		return bonusType switch
		{
			BonusType.increaseDropValue => SpawnerMaxAmountMultipliers, 
			BonusType.increaseProductionValue => SpawnerProductionMultipliers, 
			BonusType.decreaseReloadTime => SpawnerReloadMultipliers, 
			BonusType.IncreaseBaseEnergy => IncreaseEnergyMultipliers, 
			BonusType.IncreaseEnergyRechargeSpeed => IncreaseEnergyRechargeSpeedMultipliers, 
			BonusType.FreeSummon => SummonMultipliers, 
			_ => throw new Exception($"Not implemented BonusType {bonusType} collection"), 
		};
	}
}
