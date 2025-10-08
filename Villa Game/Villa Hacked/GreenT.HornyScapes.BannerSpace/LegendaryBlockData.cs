using GreenT.HornyScapes.Lootboxes;

namespace GreenT.HornyScapes.BannerSpace;

public class LegendaryBlockData
{
	public readonly RewType[] RewardsType;

	public readonly string[] RewardsValueQty;

	public readonly int[] RewardsChances;

	public readonly int[] NewMarkers;

	public readonly int[] MainMarkers;

	public LegendaryBlockData(RewType[] rewardsType, string[] rewardsValueQty, int[] rewardsChances, int[] newMarkers, int[] mainMarkers)
	{
		RewardsType = rewardsType;
		RewardsValueQty = rewardsValueQty;
		RewardsChances = rewardsChances;
		NewMarkers = newMarkers;
		MainMarkers = mainMarkers;
	}
}
