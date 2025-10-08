using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Events;
using Merge.Meta.RoomObjects;
using StripClub.Model;
using StripClub.Model.Shop;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.BattlePassSpace;

[Serializable]
public class BattlePass : IBundleProvider<BattlePassBundleData>, IRewardHolder, IDisposable
{
	public struct ViewSettings
	{
		public Sprite AnnouncementBackground;

		public Sprite AnnouncementTitleBackground;

		public Sprite Currency;

		public Sprite PurchaseWindow;

		public Sprite LevelButton;

		public Sprite PremiumTrackArrow;

		public Sprite ProgressGirl;

		public Sprite HeaderImage;

		public Sprite LeftGirl;

		public Sprite RightGirl;

		public Sprite RewardPreview;

		public Sprite StartWindowBackground;

		public Sprite RightReward;
	}

	public ReactiveProperty<bool> PremiumPurchaseProperty;

	public ILocker PremiumPurchasedLocker;

	private IDisposable _anyRewardUpdateStream;

	private readonly Dictionary<int, int> _progressPrice;

	private readonly Subject<RewardWithManyConditions> _onRewardUpdate = new Subject<RewardWithManyConditions>();

	public int ID { get; }

	public ViewSettings CurrentViewSettings { get; }

	public BattlePassBundleData Bundle { get; }

	public BattlePassDataCase Data { get; } = new BattlePassDataCase();


	public IEnumerable<ILotMonetizationData> PremiumLots { get; }

	public LevelRewardContainer AllRewardContainer { get; }

	public LevelRewardContainer FreeRewardContainer { get; }

	public LevelRewardContainer PremiumRewardContainer { get; }

	public IObservable<RewardWithManyConditions> OnRewardUpdate => _onRewardUpdate.AsObservable();

	public BattlePass(int id, Dictionary<int, int> progressPrice, ReactiveProperty<bool> unlockProperty, ViewSettings viewSettings, List<(int, RewardWithManyConditions)> freeRewards, List<(int, RewardWithManyConditions)> premiumRewards, BattlePassBundleData bundleData, IEnumerable<ILotMonetizationData> premiumLots, ILocker premiumPurchasedLocker)
	{
		ID = id;
		_progressPrice = progressPrice;
		CurrentViewSettings = viewSettings;
		PremiumPurchaseProperty = unlockProperty;
		FreeRewardContainer = new LevelRewardContainer(freeRewards);
		PremiumRewardContainer = new LevelRewardContainer(premiumRewards);
		AllRewardContainer = new LevelRewardContainer(freeRewards.Union(premiumRewards));
		PremiumPurchasedLocker = premiumPurchasedLocker;
		Bundle = bundleData;
		PremiumLots = premiumLots;
	}

	public int GetPointsToLevelUp(int currentPoints, int addLevels)
	{
		int levelForPoints = GetLevelForPoints(currentPoints);
		int key = Math.Min(_progressPrice.Last().Key, levelForPoints + addLevels);
		int num = _progressPrice[key];
		int num2 = _progressPrice[levelForPoints];
		return num - currentPoints + (currentPoints - num2);
	}

	public int GetLevelForPoints(int points)
	{
		KeyValuePair<int, int> keyValuePair = _progressPrice.FirstOrDefault((KeyValuePair<int, int> p) => p.Value > points);
		if (keyValuePair.Key != 0)
		{
			return keyValuePair.Key - 1;
		}
		return _progressPrice.Last().Key;
	}

	public int GetPointsForLevel(int level)
	{
		if (_progressPrice.ContainsKey(level))
		{
			return _progressPrice[level];
		}
		return int.MaxValue;
	}

	public int GetPointsForMaxLevel()
	{
		return _progressPrice.Last().Value;
	}

	public void Initialization()
	{
		AllRewardContainer.Initialize();
		FreeRewardContainer.Initialize();
		PremiumRewardContainer.Initialize();
		IObservable<RewardWithManyConditions> source = AllRewardContainer.Rewards.Select((RewardWithManyConditions _reward) => _reward.OnUpdate).Merge();
		_anyRewardUpdateStream = source.Subscribe(delegate(RewardWithManyConditions reward)
		{
			_onRewardUpdate.OnNext(reward);
		});
	}

	public IEnumerable<LinkedContent> GetAllRewardsContent()
	{
		return AllRewardContainer.Rewards.Select((RewardWithManyConditions reward) => reward.Content);
	}

	public virtual IEnumerable<LinkedContent> GetUncollectedRewardsContent()
	{
		return (from _reward in GetUncollectedRewards()
			select _reward.Content).ToArray();
	}

	public void TryCollectAllRewards()
	{
		foreach (RewardWithManyConditions uncollectedReward in GetUncollectedRewards())
		{
			uncollectedReward.TryCollectReward();
		}
	}

	public int GetFilteredRewardsCount(IEnumerable<EntityStatus> states)
	{
		return AllRewardContainer.Rewards.Count((RewardWithManyConditions _reward) => states.Contains(_reward.State.Value));
	}

	public bool HasRewards()
	{
		return AllRewardContainer.Rewards.Any((RewardWithManyConditions _rew) => _rew.State.Value == EntityStatus.Complete);
	}

	public bool HasUncollectedRewards()
	{
		return 0 < GetUncollectedRewards().Count();
	}

	public void Dispose()
	{
		foreach (RewardWithManyConditions reward in AllRewardContainer.Rewards)
		{
			reward.Dispose();
		}
		AllRewardContainer.Dispose();
		FreeRewardContainer.Dispose();
		PremiumRewardContainer.Dispose();
		_onRewardUpdate?.OnCompleted();
		_onRewardUpdate?.Dispose();
		_anyRewardUpdateStream?.Dispose();
	}

	private IEnumerable<RewardWithManyConditions> GetUncollectedRewards()
	{
		return AllRewardContainer.Rewards.Where((RewardWithManyConditions _reward) => _reward.State.Value == EntityStatus.Complete);
	}
}
