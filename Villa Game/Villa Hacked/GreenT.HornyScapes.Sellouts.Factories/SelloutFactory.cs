using System.Collections.Generic;
using GreenT.Data;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.Monetization;
using GreenT.HornyScapes.Sellouts.Mappers;
using GreenT.HornyScapes.Sellouts.Models;
using GreenT.HornyScapes.Sellouts.Providers;
using StripClub.Model;
using Zenject;

namespace GreenT.HornyScapes.Sellouts.Factories;

public class SelloutFactory : IFactory<SelloutMapper, Sellout>, IFactory
{
	private const string RewardSaveKey = "sellout.{0}.reward.{1}";

	private readonly ISaver _saver;

	private readonly SignalBus _signalBus;

	private readonly LockerFactory _lockerFactory;

	private readonly SelloutRewardsMapperProvider _selloutRewardsInfoManager;

	private readonly SelloutRewardFactory _selloutRewardFactory;

	private readonly IPurchaseNotifier _monetizationSystem;

	public SelloutFactory(ISaver saver, IPurchaseNotifier monetizationSystem, LockerFactory lockerFactory, SignalBus signalBus, SelloutRewardsMapperProvider selloutRewardsInfoManager, SelloutRewardFactory selloutRewardFactory)
	{
		_saver = saver;
		_monetizationSystem = monetizationSystem;
		_lockerFactory = lockerFactory;
		_signalBus = signalBus;
		_selloutRewardsInfoManager = selloutRewardsInfoManager;
		_selloutRewardFactory = selloutRewardFactory;
	}

	public Sellout Create(SelloutMapper mapper)
	{
		List<int> requirementPoints = new List<int>(32);
		List<SelloutRewardsInfo> rewards = CreateRewards(mapper, requirementPoints);
		Sellout sellout = new Sellout(mapper.id, mapper.bundle, mapper.go_to, requirementPoints, rewards);
		_saver.Add(sellout);
		return sellout;
	}

	private List<SelloutRewardsInfo> CreateRewards(SelloutMapper selloutMapper, List<int> requirementPoints)
	{
		int[] rewards_id = selloutMapper.rewards_id;
		int id = selloutMapper.id;
		int num = rewards_id.Length;
		List<SelloutRewardsInfo> list = new List<SelloutRewardsInfo>(num);
		for (int i = 0; i < num; i++)
		{
			SelloutRewardsMapper selloutRewardsMapper = _selloutRewardsInfoManager.Get(rewards_id[i]);
			int points_price = selloutRewardsMapper.points_price;
			string saveKey = $"sellout.{id}.reward.{i}";
			IReadOnlyList<RewardWithManyConditions> premiumRewards = _selloutRewardFactory.Create(selloutRewardsMapper, saveKey, id, points_price, isPremium: true);
			IReadOnlyList<RewardWithManyConditions> rewards = _selloutRewardFactory.Create(selloutRewardsMapper, saveKey, id, points_price, isPremium: false);
			SelloutRewardsInfo item = new SelloutRewardsInfo(premiumRewards, rewards);
			list.Add(item);
			requirementPoints.Add(points_price);
		}
		return list;
	}
}
