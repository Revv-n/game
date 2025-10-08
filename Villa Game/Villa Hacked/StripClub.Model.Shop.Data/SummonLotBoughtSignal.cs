namespace StripClub.Model.Shop.Data;

public class SummonLotBoughtSignal : LotBoughtSignal
{
	public readonly bool Wholesale;

	public SummonLotBoughtSignal(Lot lot, bool wholesale)
		: base(lot)
	{
		Wholesale = wholesale;
	}
}
