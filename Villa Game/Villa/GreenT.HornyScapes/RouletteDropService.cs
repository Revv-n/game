using GreenT.HornyScapes.MiniEvents;
using StripClub.Model;
using UnityEngine;

namespace GreenT.HornyScapes;

public class RouletteDropService
{
	private readonly GarantChance _garantChance;

	private readonly RouletteLot.RewardSettings _rewardSettings;

	private const int MIN_WEIGHT = 0;

	private const int MAX_WEIGHT = 100;

	public RouletteDropService(GarantChance garantChance, RouletteLot.RewardSettings rewardSettings)
	{
		_garantChance = garantChance;
		_rewardSettings = rewardSettings;
	}

	public bool TryGetContent(int rollQuantity, out LinkedContent linkedContent)
	{
		int chance = _garantChance.GetChance(rollQuantity);
		int num = Random.Range(0, 100);
		linkedContent = ((chance > num) ? _rewardSettings.UniqueReward.Clone() : _rewardSettings.DefaultReward.Clone());
		return chance > num;
	}
}
