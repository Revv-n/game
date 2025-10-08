using System.Collections.Generic;
using GreenT.HornyScapes.Data;
using StripClub.Model.Shop;
using StripClub.Model.Shop.Data;
using Zenject;

namespace GreenT.HornyScapes.Bank.Data;

public class BundleLotInitializer : StructureInitializerViaArray<ShopBundleMapper, BundleLot, Lot>
{
	public BundleLotInitializer(IManager<Lot> manager, IFactory<ShopBundleMapper, BundleLot> factory, IEnumerable<IStructureInitializer> others = null)
		: base(manager, factory, others)
	{
	}
}
