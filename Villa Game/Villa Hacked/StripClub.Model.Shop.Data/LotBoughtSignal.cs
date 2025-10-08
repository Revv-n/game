namespace StripClub.Model.Shop.Data;

public class LotBoughtSignal
{
	public Lot Lot { get; }

	public LotBoughtSignal(Lot lot)
	{
		Lot = lot;
	}
}
