namespace StripClub.Model.Shop;

public class LotFeatures
{
	public Stickers Stickers { get; }

	public int? SaleValue { get; }

	public LotFeatures(Stickers stickers, int? saleValue)
	{
		Stickers = stickers;
		SaleValue = saleValue;
	}

	public LotFeatures(bool hot, bool best, bool sale, int? saleValue)
	{
		Stickers = (Stickers)0;
		if (hot)
		{
			Stickers |= Stickers.Hot;
		}
		if (best)
		{
			Stickers |= Stickers.Best;
		}
		if (sale)
		{
			Stickers |= Stickers.Sale;
			SaleValue = saleValue;
		}
	}
}
