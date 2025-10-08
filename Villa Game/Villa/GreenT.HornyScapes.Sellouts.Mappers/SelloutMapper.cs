using GreenT.HornyScapes.Data;
using GreenT.HornyScapes.Events;

namespace GreenT.HornyScapes.Sellouts.Mappers;

[Mapper]
public class SelloutMapper : IEventMapper
{
	public int id;

	public int[] rewards_id;

	public string bundle;

	public int go_to;

	public int ID => id;

	public string Bundle => bundle;
}
