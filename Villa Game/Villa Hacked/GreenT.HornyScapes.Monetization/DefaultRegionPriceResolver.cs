using GreenT.HornyScapes.Monetization.Windows;
using StripClub.Model.Shop;

namespace GreenT.HornyScapes.Monetization;

public class DefaultRegionPriceResolver : BasePriceResolver
{
	private readonly string _setupRegion;

	public DefaultRegionPriceResolver(string setupRegion, LotManager lotManager, LocalizedPriceManager localizedPriceManager)
		: base(lotManager, localizedPriceManager)
	{
		_setupRegion = setupRegion;
	}

	public override void Initialize()
	{
		Setup(_setupRegion);
	}
}
