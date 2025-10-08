using System.Collections.Generic;
using GreenT.HornyScapes.Data;
using Zenject;

namespace GreenT.HornyScapes.StarShop.Data;

public class StarShopStructureInitializer : StructureInitializerViaArray<StarShopMapper, StarShopItem>
{
	public StarShopStructureInitializer(IManager<StarShopItem> manager, IFactory<StarShopMapper, StarShopItem> factory, IEnumerable<IStructureInitializer> others = null)
		: base(manager, factory, others)
	{
	}
}
