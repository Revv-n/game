using GreenT.HornyScapes.Lootboxes;

namespace GreenT.HornyScapes.BannerSpace;

public class RareBlockData
{
	public RewType[] RewardsType;

	public string[] RewardsValueQty;

	public int[] RewardsChances;

	public RareBlockData(RewType[] rewardsType, string[] rewardsValueQty, int[] rewardsChances)
	{
		RewardsType = rewardsType;
		RewardsValueQty = rewardsValueQty;
		RewardsChances = rewardsChances;
	}
}
