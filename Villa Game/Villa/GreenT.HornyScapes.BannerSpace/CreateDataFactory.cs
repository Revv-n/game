using StripClub.Model;
using Zenject;

namespace GreenT.HornyScapes.BannerSpace;

public class CreateDataFactory : IFactory<BannerMapper, CreateData>, IFactory
{
	private readonly LockerFactory _lockerFactory;

	public CreateDataFactory(LockerFactory lockerFactory)
	{
		_lockerFactory = lockerFactory;
	}

	public CreateData Create(BannerMapper mapper)
	{
		BannerInfoData info = CreateInfo(mapper);
		BannerCostData cost = CreateCost(mapper);
		BannerMainRewardData mainReward = CreateMainReward(mapper);
		BannerChancesData chances = CreateChances(mapper);
		LegendaryBlockData legendaryBlock = CreateLegendaryBlock(mapper);
		EpicBlockData epicBlock = CreateEpicBlock(mapper);
		RareBlockData rareBlock = CreateRareBlock(mapper);
		BannerUnlockData unlock = CreateUnlock(mapper);
		return new CreateData(info, cost, mainReward, chances, legendaryBlock, epicBlock, rareBlock, unlock, mapper.Type);
	}

	private BannerInfoData CreateInfo(BannerMapper mapper)
	{
		return new BannerInfoData(mapper.id, mapper.source, mapper.bank_tab_id, mapper.banner_group, mapper.content_source, mapper.background_name);
	}

	private BannerCostData CreateCost(BannerMapper mapper)
	{
		return new BannerCostData(mapper.buy_resource, mapper.price_x1, mapper.price_x10, mapper.rebuy_resource, mapper.rebuy_cost_1x);
	}

	private BannerMainRewardData CreateMainReward(BannerMapper mapper)
	{
		return new BannerMainRewardData(mapper.main_reward_id, mapper.legendary_reward_id, mapper.epic_reward_id, mapper.rare_reward_id);
	}

	private BannerChancesData CreateChances(BannerMapper mapper)
	{
		return new BannerChancesData(mapper.main_reward_chances, mapper.epic_reward_chances, mapper.garant_id);
	}

	private LegendaryBlockData CreateLegendaryBlock(BannerMapper mapper)
	{
		BannerMapper bannerMapper = mapper;
		if (bannerMapper.details_legendary_new == null)
		{
			bannerMapper.details_legendary_new = new int[mapper.details_legendary_rewards_type.Length];
		}
		bannerMapper = mapper;
		if (bannerMapper.details_legendary_main == null)
		{
			bannerMapper.details_legendary_main = new int[mapper.details_legendary_rewards_type.Length];
		}
		return new LegendaryBlockData(mapper.details_legendary_rewards_type, mapper.details_legendary_rewards_value_qty, mapper.details_legendary_rewards_chances, mapper.details_legendary_new, mapper.details_legendary_main);
	}

	private EpicBlockData CreateEpicBlock(BannerMapper mapper)
	{
		return new EpicBlockData(mapper.details_epic_rewards_type, mapper.details_epic_rewards_value_qty, mapper.details_epic_rewards_chances);
	}

	private RareBlockData CreateRareBlock(BannerMapper mapper)
	{
		return new RareBlockData(mapper.details_rare_rewards_type, mapper.details_rare_rewards_value_qty, mapper.details_rare_rewards_chances);
	}

	private BannerUnlockData CreateUnlock(BannerMapper mapper)
	{
		return new BannerUnlockData(LockerFactory.CreateFromParamsArray(mapper.unlock_type, mapper.unlock_value, _lockerFactory, LockerSourceType.Banner));
	}
}
