using System.Numerics;
using GreenT.Types;
using StripClub.Model;

namespace StripClub.UI;

public interface IResourceView
{
	CurrencyType Resource { get; }

	BigInteger Count { get; }

	void Set(CurrencyType currency, BigInteger Count, CompositeIdentificator compositeIdentificator);
}
