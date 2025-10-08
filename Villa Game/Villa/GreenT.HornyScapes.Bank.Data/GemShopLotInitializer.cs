using System.Collections.Generic;
using GreenT.HornyScapes.Data;
using StripClub.Model.Shop;
using StripClub.Model.Shop.Data;
using Zenject;

namespace GreenT.HornyScapes.Bank.Data;

public class GemShopLotInitializer : StructureInitializerViaArray<GemShopMapper, GemShopLot, Lot>
{
	public GemShopLotInitializer(IManager<Lot> manager, IFactory<GemShopMapper, GemShopLot> factory, IEnumerable<IStructureInitializer> others = null)
		: base(manager, factory, others)
	{
	}
}
