using System.Collections.Generic;
using GreenT.HornyScapes.Data;
using GreenT.HornyScapes.StarShop.Story;
using Zenject;

namespace GreenT.HornyScapes.StarShop.Data;

public class StarShopArtStructureInitializer : StructureInitializerViaArray<StarShopArtMapper, StarShopArt>
{
	public StarShopArtStructureInitializer(IManager<StarShopArt> manager, IFactory<StarShopArtMapper, StarShopArt> factory, IEnumerable<IStructureInitializer> others = null)
		: base(manager, factory, others)
	{
	}
}
