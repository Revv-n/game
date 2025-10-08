using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.Events.BattlePassRewardCards;
using Merge.Meta.RoomObjects;
using ModestTree;

namespace GreenT.HornyScapes.BattlePassSpace.RewardCards;

public class BattlePassRewardPairData
{
	public readonly int StartProgressValue;

	public readonly int TargetProgressValue;

	public readonly int TargetLevel;

	public readonly int StartLevel;

	public readonly bool IsLast;

	public readonly IEnumerable<RewardWithManyConditions> FreeReward;

	public readonly IEnumerable<RewardWithManyConditions> PremiumReward;

	public EntityStatus Status => GetStatus();

	public BattlePassRewardViewType GetViewType()
	{
		if (PremiumReward.Count() > 1 || FreeReward.Count() > 1)
		{
			return BattlePassRewardViewType.Simple;
		}
		if (!LinqExtensions.IsEmpty<RewardWithManyConditions>(FreeReward) && !LinqExtensions.IsEmpty<RewardWithManyConditions>(PremiumReward) && PremiumReward.First() == FreeReward.First())
		{
			return BattlePassRewardViewType.Merged;
		}
		return BattlePassRewardViewType.Simple;
	}

	public BattlePassRewardPairData(BattlePass battlePass, int startProgressLevel, int targetProgressLevel, bool isLast)
	{
		PremiumReward = battlePass.PremiumRewardContainer.GetRewardsForLevel(targetProgressLevel);
		FreeReward = battlePass.FreeRewardContainer.GetRewardsForLevel(targetProgressLevel);
		IsLast = isLast;
		TargetLevel = targetProgressLevel;
		StartLevel = startProgressLevel;
		StartProgressValue = battlePass.GetPointsForLevel(startProgressLevel);
		TargetProgressValue = battlePass.GetPointsForLevel(targetProgressLevel);
	}

	public bool HasAnyRewardComplete()
	{
		if (!TryGetState(FreeReward, out var targetState) || targetState != EntityStatus.Complete)
		{
			if (TryGetState(PremiumReward, out var targetState2))
			{
				return targetState2 == EntityStatus.Complete;
			}
			return false;
		}
		return true;
	}

	private bool IsState(IEnumerable<RewardWithManyConditions> rewards, EntityStatus state)
	{
		return rewards.All((RewardWithManyConditions reward) => reward.State.Value == state);
	}

	private bool TryGetState(IEnumerable<RewardWithManyConditions> rewards, out EntityStatus targetState)
	{
		List<RewardWithManyConditions> list = rewards.ToList();
		targetState = EntityStatus.Blocked;
		if (LinqExtensions.IsEmpty<RewardWithManyConditions>((IEnumerable<RewardWithManyConditions>)list))
		{
			return false;
		}
		targetState = list.First().State.Value;
		foreach (RewardWithManyConditions item in list)
		{
			if (item.State.Value != targetState)
			{
				return false;
			}
		}
		return true;
	}

	private EntityStatus GetStatus()
	{
		if (LinqExtensions.IsEmpty<RewardWithManyConditions>(PremiumReward) && TryGetState(FreeReward, out var targetState))
		{
			return targetState;
		}
		if (LinqExtensions.IsEmpty<RewardWithManyConditions>(FreeReward) && TryGetState(PremiumReward, out targetState))
		{
			return targetState;
		}
		if (TryGetState(PremiumReward, out targetState) && IsState(FreeReward, targetState))
		{
			return targetState;
		}
		if (TryGetState(FreeReward, out targetState) && IsState(PremiumReward, targetState))
		{
			return targetState;
		}
		if (IsState(FreeReward, EntityStatus.Blocked) || IsState(PremiumReward, EntityStatus.Blocked))
		{
			return EntityStatus.Blocked;
		}
		return EntityStatus.InProgress;
	}
}
