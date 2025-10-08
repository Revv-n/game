using GreenT.Types;

namespace StripClub.Model.Shop;

public class IntPrice : Price<int>
{
	public IntPrice(int price, CurrencyType currency, CompositeIdentificator compositeIdentificator)
		: base(price, currency, compositeIdentificator)
	{
	}
}
