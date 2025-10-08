using GreenT.Types;

namespace StripClub.Model.Shop;

public class FloatPrice : Price<float>
{
	public FloatPrice(float price, CurrencyType currency, CompositeIdentificator compositeIdentificator)
		: base(price, currency, compositeIdentificator)
	{
	}
}
