using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.Data;
using GreenT.HornyScapes.Events;
using Merge.Meta.RoomObjects;
using StripClub.Model;
using StripClub.Model.Shop;
using UniRx;

namespace GreenT.HornyScapes.Sellouts.Models;

[Serializable]
[MementoHolder]
public class Sellout : ISavableState, IRewardHolder
{
	[Serializable]
	public class SelloutMemento : Memento
	{
		public EntityStatus[] RewardsStates;

		public EntityStatus[] PremiumRewardsStates;

		public int Points;

		public bool IsFirstShowed;

		public bool CanPointsTrack;

		public SelloutMemento(Sellout savableState)
			: base(savableState)
		{
			List<SelloutRewardsInfo> rewards = savableState.Rewards;
			RewardsStates = rewards.Select((SelloutRewardsInfo rewardInfo) => rewardInfo.Rewards[0].State.Value).ToArray();
			IEnumerable<RewardWithManyConditions> source = from rewardInfo in rewards
				where rewardInfo.PremiumRewards != null && 0 < rewardInfo.PremiumRewards.Count
				select rewardInfo.PremiumRewards[0];
			PremiumRewardsStates = (source.Any() ? source.Select((RewardWithManyConditions reward) => reward.State.Value).ToArray() : Array.Empty<EntityStatus>());
			Points = savableState.Points.Value;
			IsFirstShowed = savableState.IsFirstShowed.Value;
			CanPointsTrack = savableState.CanPointsTrack.Value;
		}
	}

	private readonly ReactiveProperty<string> _uniqueKey;

	private readonly ReactiveProperty<int> _points = new ReactiveProperty<int>(0);

	private readonly ReactiveProperty<bool> _canPointsTrack = new ReactiveProperty<bool>();

	private readonly ReactiveProperty<bool> _isFirstShowed = new ReactiveProperty<bool>();

	public int ID { get; }

	public string Bundle { get; }

	public int GoToBankTab { get; }

	public List<int> RequirementPoints { get; }

	public List<SelloutRewardsInfo> Rewards { get; }

	public SelloutBundleData BundleData { get; private set; }

	public IReadOnlyReactiveProperty<int> Points => (IReadOnlyReactiveProperty<int>)(object)_points;

	public IReadOnlyReactiveProperty<bool> CanPointsTrack => (IReadOnlyReactiveProperty<bool>)(object)_canPointsTrack;

	public IReadOnlyReactiveProperty<bool> IsFirstShowed => (IReadOnlyReactiveProperty<bool>)(object)_isFirstShowed;

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public Sellout(int id, string bundle, int go_to, List<int> requirementPoints, List<SelloutRewardsInfo> rewards)
	{
		ID = id;
		Bundle = bundle;
		GoToBankTab = go_to;
		RequirementPoints = requirementPoints;
		Rewards = rewards;
		_uniqueKey = new ReactiveProperty<string>
		{
			Value = $"sellout.new.{ID}"
		};
		_canPointsTrack.Value = true;
	}

	public void Set(SelloutBundleData bundleData)
	{
		BundleData = bundleData;
	}

	public bool HasCompleteReward()
	{
		return Rewards.Any((SelloutRewardsInfo rewards) => HasCompleteReward(rewards.PremiumRewards) || HasCompleteReward(rewards.Rewards));
	}

	public bool ValidateBought(Lot lot)
	{
		LogValidateBought(lot);
		return lot.IsReal;
	}

	public void AddPoints(int points)
	{
		ReactiveProperty<int> points2 = _points;
		points2.Value += points;
	}

	public void StopTrackPoints()
	{
		_canPointsTrack.Value = false;
	}

	public IEnumerable<LinkedContent> GetAllRewardsContent()
	{
		return from reward in Rewards.SelectMany((SelloutRewardsInfo rewardInfo) => rewardInfo.PremiumRewards.Concat(rewardInfo.Rewards))
			select reward.Content;
	}

	public virtual IEnumerable<LinkedContent> GetUncollectedRewardsContent()
	{
		return from reward in Rewards.SelectMany((SelloutRewardsInfo rewardInfo) => rewardInfo.PremiumRewards.Concat(rewardInfo.Rewards))
			where reward.State.Value == EntityStatus.Rewarded
			select reward.Content;
	}

	public int GetUncollectedRewardsCount()
	{
		return Rewards.SelectMany((SelloutRewardsInfo rewardInfo) => rewardInfo.PremiumRewards.Concat(rewardInfo.Rewards)).Count((RewardWithManyConditions reward) => reward.State.Value == EntityStatus.Rewarded);
	}

	public int GetFilteredRewardsCount(IEnumerable<EntityStatus> states)
	{
		return Rewards.SelectMany((SelloutRewardsInfo rewardInfo) => rewardInfo.PremiumRewards.Concat(rewardInfo.Rewards)).Count((RewardWithManyConditions reward) => states.Contains(reward.State.Value));
	}

	public bool HasRewards()
	{
		return Rewards.SelectMany((SelloutRewardsInfo rewardInfo) => rewardInfo.PremiumRewards.Concat(rewardInfo.Rewards)).Any((RewardWithManyConditions reward) => reward.State.Value == EntityStatus.Rewarded);
	}

	public void SetShowed()
	{
		_isFirstShowed.Value = true;
	}

	public void Reset()
	{
		_points.Value = 0;
		_isFirstShowed.Value = false;
		_canPointsTrack.Value = true;
	}

	private void LogValidateBought(Lot lot)
	{
	}

	private bool HasCompleteReward(IReadOnlyList<RewardWithManyConditions> targetRewards)
	{
		return targetRewards.Any((RewardWithManyConditions reward) => reward.State.Value == EntityStatus.Complete);
	}

	public string UniqueKey()
	{
		return _uniqueKey.Value;
	}

	public IReadOnlyReactiveProperty<string> GetSaveKey()
	{
		return (IReadOnlyReactiveProperty<string>)(object)_uniqueKey;
	}

	public Memento SaveState()
	{
		return new SelloutMemento(this);
	}

	public void LoadState(Memento memento)
	{
		if (memento is MigrationDummyMemento)
		{
			return;
		}
		SelloutMemento selloutMemento = (SelloutMemento)memento;
		_points.Value = selloutMemento.Points;
		_isFirstShowed.Value = selloutMemento.IsFirstShowed;
		_canPointsTrack.Value = selloutMemento.CanPointsTrack;
		EntityStatus[] rewardsStates = selloutMemento.RewardsStates;
		EntityStatus[] premiumRewardsStates = selloutMemento.PremiumRewardsStates;
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < Rewards.Count; i++)
		{
			SelloutRewardsInfo selloutRewardsInfo = Rewards[i];
			IReadOnlyList<RewardWithManyConditions> rewards = selloutRewardsInfo.Rewards;
			IReadOnlyList<RewardWithManyConditions> premiumRewards = selloutRewardsInfo.PremiumRewards;
			for (int j = 0; j < rewards.Count; j++)
			{
				rewards[j].State.Value = rewardsStates[num];
			}
			for (int k = 0; k < premiumRewards.Count; k++)
			{
				premiumRewards[k].State.Value = premiumRewardsStates[num2];
			}
			if (0 < rewards.Count)
			{
				num++;
			}
			if (0 < premiumRewards.Count)
			{
				num2++;
			}
		}
	}
}
