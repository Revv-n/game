namespace StripClub.Model;

public class CurrencyItem : Item
{
	public new CurrencyItemInfo Info => base.Info as CurrencyItemInfo;

	public CurrencyItem(CurrencyItemInfo item, int amount)
		: base(item, amount)
	{
	}
}
