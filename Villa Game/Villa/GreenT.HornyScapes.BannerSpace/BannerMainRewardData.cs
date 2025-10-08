namespace GreenT.HornyScapes.BannerSpace;

public class BannerMainRewardData
{
	public readonly int MainRewardId;

	public readonly int LegendaryRewardId;

	public readonly int EpicRewardId;

	public readonly int RareRewardId;

	public BannerMainRewardData(int mainRewardId, int legendaryRewardId, int epicRewardId, int rareRewardId)
	{
		MainRewardId = mainRewardId;
		LegendaryRewardId = legendaryRewardId;
		EpicRewardId = epicRewardId;
		RareRewardId = rareRewardId;
	}
}
