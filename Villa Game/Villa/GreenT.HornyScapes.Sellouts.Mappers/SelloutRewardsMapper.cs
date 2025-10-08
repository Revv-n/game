using GreenT.HornyScapes.Data;
using GreenT.HornyScapes.Lootboxes;

namespace GreenT.HornyScapes.Sellouts.Mappers;

[Mapper]
public class SelloutRewardsMapper
{
	public int id;

	public int points_price;

	public int frame_id;

	public RewType[] rewards_type;

	public string[] rewards_id;

	public int[] rewards_qty;

	public RewType[] premium_type;

	public string[] premium_id;

	public int[] premium_qty;
}
