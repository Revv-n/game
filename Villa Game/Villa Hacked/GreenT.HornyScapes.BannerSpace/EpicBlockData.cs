using GreenT.HornyScapes.Lootboxes;

namespace GreenT.HornyScapes.BannerSpace;

public class EpicBlockData
{
	public readonly RewType[] RewardsType;

	public readonly string[] RewardsValueQty;

	public readonly int[] RewardsChances;

	public EpicBlockData(RewType[] rewardsType, string[] rewardsValueQty, int[] rewardsChances)
	{
		RewardsType = rewardsType;
		RewardsValueQty = rewardsValueQty;
		RewardsChances = rewardsChances;
	}
}
