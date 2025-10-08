using System.Collections.Generic;
using GreenT.HornyScapes.Data;
using Zenject;

namespace GreenT.HornyScapes.Lootboxes.Data;

public class LootboxStructureInitializer : StructureInitializerViaArray<LootboxMapper, Lootbox>
{
	public LootboxStructureInitializer(IManager<Lootbox> manager, IFactory<LootboxMapper, Lootbox> factory, IEnumerable<IStructureInitializer> others = null)
		: base(manager, factory, others)
	{
	}
}
