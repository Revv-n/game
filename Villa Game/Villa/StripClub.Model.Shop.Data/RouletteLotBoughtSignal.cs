using GreenT.HornyScapes;

namespace StripClub.Model.Shop.Data;

public class RouletteLotBoughtSignal
{
	public readonly bool Wholesale;

	public RouletteLot Lot { get; }

	public RouletteLotBoughtSignal(RouletteLot lot, bool wholesale)
	{
		Wholesale = wholesale;
		Lot = lot;
	}
}
