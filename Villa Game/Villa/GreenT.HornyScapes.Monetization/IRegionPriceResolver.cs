using System.Collections.Generic;

namespace GreenT.HornyScapes.Monetization;

public interface IRegionPriceResolver
{
	string CurrentRegion { get; }

	IDictionary<string, decimal> Prices { get; }

	void Initialize();

	void UpdatePrices(string region);

	decimal GetPriceConvertedToUS(string paymentID);
}
