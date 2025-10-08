using System.Collections.Generic;
using GreenT.HornyScapes.Events;

namespace GreenT.HornyScapes.Sellouts.Models;

public sealed class SelloutRewardsInfo
{
	public IReadOnlyList<RewardWithManyConditions> PremiumRewards { get; }

	public IReadOnlyList<RewardWithManyConditions> Rewards { get; }

	public SelloutRewardsInfo(IReadOnlyList<RewardWithManyConditions> premiumRewards, IReadOnlyList<RewardWithManyConditions> rewards)
	{
		PremiumRewards = premiumRewards;
		Rewards = rewards;
	}
}
