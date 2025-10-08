using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.Data;
using GreenT.HornyScapes.Events;
using Merge.Meta.RoomObjects;

namespace GreenT.HornyScapes.BattlePassSpace.Data;

[Serializable]
[MementoHolder]
public class BattlePassRewardDataLogics : ISavableState
{
	[Serializable]
	public class BattlePassRewardMemento : Memento
	{
		public Dictionary<string, EntityStatus> AllRewardsSave;

		public BattlePassRewardMemento(BattlePassRewardDataLogics logics)
			: base(logics)
		{
			AllRewardsSave = null;
			IEnumerable<RewardWithManyConditions> enumerable = logics.allRewards.Where((RewardWithManyConditions p) => IsTarget(p.State.Value));
			AllRewardsSave = new Dictionary<string, EntityStatus>(enumerable.Count());
			foreach (RewardWithManyConditions item in enumerable)
			{
				AllRewardsSave.Add(item.SaveKey, item.State.Value);
			}
		}
	}

	private readonly IEnumerable<RewardWithManyConditions> allRewards;

	private readonly string id;

	public bool IsLoaded { get; private set; }

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public BattlePassRewardDataLogics(BattlePass battlePass)
	{
		allRewards = battlePass.AllRewardContainer.Rewards;
		id = $"battle_pass_{battlePass.ID}";
	}

	public string UniqueKey()
	{
		return id;
	}

	public Memento SaveState()
	{
		return new BattlePassRewardMemento(this);
	}

	public void LoadState(Memento memento)
	{
		Dictionary<string, EntityStatus> allRewardsSave = ((BattlePassRewardMemento)memento).AllRewardsSave;
		foreach (RewardWithManyConditions allReward in allRewards)
		{
			if (allRewardsSave.ContainsKey(allReward.SaveKey))
			{
				EntityStatus status = allRewardsSave[allReward.SaveKey];
				if (IsTarget(status))
				{
					allReward.ForceSetState(status);
				}
			}
		}
		IsLoaded = true;
	}

	private static bool IsTarget(EntityStatus status)
	{
		if (status != EntityStatus.Rewarded)
		{
			return status == EntityStatus.Complete;
		}
		return true;
	}

	public void Reset()
	{
		foreach (RewardWithManyConditions allReward in allRewards)
		{
			allReward.Dispose();
			allReward.ForceSetState(EntityStatus.Blocked);
		}
	}
}
