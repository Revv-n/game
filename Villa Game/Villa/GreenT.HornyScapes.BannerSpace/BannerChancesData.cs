namespace GreenT.HornyScapes.BannerSpace;

public class BannerChancesData
{
	public readonly int[] MainRewardChances;

	public readonly int[] EpicRewardChances;

	public readonly int GarantId;

	public BannerChancesData(int[] mainRewardChances, int[] epicRewardChances, int garantId)
	{
		MainRewardChances = mainRewardChances;
		EpicRewardChances = epicRewardChances;
		GarantId = garantId;
	}
}
