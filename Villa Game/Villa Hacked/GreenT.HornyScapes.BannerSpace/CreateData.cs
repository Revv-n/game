using GreenT.Types;

namespace GreenT.HornyScapes.BannerSpace;

public class CreateData
{
	public readonly BannerInfoData Info;

	public readonly BannerCostData Cost;

	public readonly BannerMainRewardData MainReward;

	public readonly BannerChancesData Chances;

	public readonly LegendaryBlockData LegendaryBlock;

	public readonly EpicBlockData EpicBlock;

	public readonly RareBlockData RareBlock;

	public readonly BannerUnlockData Unlock;

	public readonly ConfigContentType Type;

	public BannerBackgroundBundle BackgroundBundle;

	public CreateData(BannerInfoData info, BannerCostData cost, BannerMainRewardData mainReward, BannerChancesData chances, LegendaryBlockData legendaryBlock, EpicBlockData epicBlock, RareBlockData rareBlock, BannerUnlockData unlock, ConfigContentType type)
	{
		Info = info;
		Cost = cost;
		MainReward = mainReward;
		Chances = chances;
		LegendaryBlock = legendaryBlock;
		EpicBlock = epicBlock;
		RareBlock = rareBlock;
		Unlock = unlock;
		Type = type;
	}

	public void SetBundle(BannerBackgroundBundle backgroundBundle)
	{
		BackgroundBundle = backgroundBundle;
	}
}
