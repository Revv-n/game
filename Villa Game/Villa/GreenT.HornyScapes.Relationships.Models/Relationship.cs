using System;
using System.Collections.Generic;
using GreenT.Data;
using GreenT.HornyScapes.Dates.Models;
using GreenT.HornyScapes.Events;
using Merge.Meta.RoomObjects;
using UniRx;

namespace GreenT.HornyScapes.Relationships.Models;

public class Relationship : ISavableState
{
	[Serializable]
	public class RelationShipMemento : Memento
	{
		public Dictionary<string, EntityStatus> RewardsState { get; }

		public List<string> ComingSoonDates { get; }

		public int RelationShipLevel { get; }

		public RelationShipMemento(Relationship relationship)
			: base(relationship)
		{
			RelationShipLevel = relationship.RelationshipLevel.Value;
			RewardsState = new Dictionary<string, EntityStatus>();
			ComingSoonDates = new List<string>();
			foreach (IReadOnlyList<RewardWithManyConditions> reward in relationship.Rewards)
			{
				foreach (RewardWithManyConditions item in reward)
				{
					RewardsState.Add(item.SaveKey, item.State.Value);
					if (item.Content is ComingSoonDateLinkedContent)
					{
						ComingSoonDates.Add(item.SaveKey);
					}
				}
			}
		}
	}

	private const string SavePrefix = "relationship.";

	private readonly int[] _rewardIds;

	private readonly IReadOnlyList<int> _rewardPointsRequirements;

	private readonly List<string> _previousComingSoonDates = new List<string>();

	private readonly Subject<int> _rewardClaimed = new Subject<int>();

	private readonly ReactiveProperty<bool> _wasComingSoonDates = new ReactiveProperty<bool>(initialValue: false);

	private readonly string _saveKey;

	public int ID { get; }

	public IReadOnlyList<IReadOnlyList<RewardWithManyConditions>> Rewards { get; }

	public ReactiveProperty<int> RelationshipLevel { get; }

	public IObservable<int> RewardClaimed => _rewardClaimed.AsObservable();

	public IReadOnlyReactiveProperty<bool> WasComingSoonDates => _wasComingSoonDates;

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public Relationship(int id, IReadOnlyList<int> rewardPointsRequirements, int[] rewardIds, IReadOnlyList<IReadOnlyList<RewardWithManyConditions>> rewards)
	{
		ID = id;
		Rewards = rewards;
		_saveKey = "relationship." + id;
		RelationshipLevel = new ReactiveProperty<int>();
		_rewardIds = rewardIds;
		_rewardPointsRequirements = rewardPointsRequirements;
	}

	public int GetRequiredPointsForReward(int index)
	{
		return _rewardPointsRequirements[index];
	}

	public void ClaimReward(int rewardId)
	{
		_rewardClaimed?.OnNext(rewardId);
	}

	public bool WasComingSoonDate(RewardWithManyConditions reward)
	{
		return _previousComingSoonDates.Contains(reward.SaveKey);
	}

	public bool RemoveComingSoonDate(RewardWithManyConditions reward)
	{
		bool num = _previousComingSoonDates.Remove(reward.SaveKey);
		if (num)
		{
			_wasComingSoonDates.Value = 0 < _previousComingSoonDates.Count;
		}
		return num;
	}

	public void AddComingSoonDate(string saveKey)
	{
		if (!_previousComingSoonDates.Contains(saveKey))
		{
			_previousComingSoonDates.Add(saveKey);
			_wasComingSoonDates.Value = true;
		}
	}

	public bool WasAnyComingSoonDate()
	{
		return 0 < _previousComingSoonDates.Count;
	}

	public void CheckReward(int rewardId)
	{
		int num = -1;
		for (int i = 0; i < _rewardIds.Length; i++)
		{
			if (rewardId == _rewardIds[i])
			{
				num = i;
				break;
			}
		}
		if (num != -1 && Rewards[num][0].State.Value == EntityStatus.Rewarded)
		{
			ClaimReward(_rewardIds[num]);
		}
	}

	private void CheckRewards()
	{
		int num = 0;
		foreach (IReadOnlyList<RewardWithManyConditions> reward in Rewards)
		{
			RewardWithManyConditions rewardWithManyConditions = reward[0];
			if (rewardWithManyConditions.Content is ComingSoonDateLinkedContent && WasComingSoonDate(rewardWithManyConditions))
			{
				RemoveComingSoonDate(rewardWithManyConditions);
			}
			if (rewardWithManyConditions.State.Value == EntityStatus.Rewarded)
			{
				ClaimReward(_rewardIds[num]);
			}
			num++;
		}
	}

	public string UniqueKey()
	{
		return _saveKey;
	}

	public Memento SaveState()
	{
		return new RelationShipMemento(this);
	}

	public void LoadState(Memento memento)
	{
		RelationShipMemento relationShipMemento = (RelationShipMemento)memento;
		RelationshipLevel.Value = relationShipMemento.RelationShipLevel;
		foreach (IReadOnlyList<RewardWithManyConditions> reward in Rewards)
		{
			foreach (RewardWithManyConditions item in reward)
			{
				if (relationShipMemento.RewardsState.TryGetValue(item.SaveKey, out var value) && (value == EntityStatus.Rewarded || value == EntityStatus.Complete))
				{
					item.ForceSetState(value);
				}
			}
		}
		foreach (string comingSoonDate in relationShipMemento.ComingSoonDates)
		{
			AddComingSoonDate(comingSoonDate);
		}
		CheckRewards();
	}
}
