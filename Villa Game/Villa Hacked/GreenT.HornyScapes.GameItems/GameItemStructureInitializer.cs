using System.Collections.Generic;
using GreenT.HornyScapes.Data;
using Merge;
using Zenject;

namespace GreenT.HornyScapes.GameItems;

public class GameItemStructureInitializer : StructureInitializerViaArray<GameItemMapper, GIConfig>
{
	public GameItemStructureInitializer(IManager<GIConfig> manager, IFactory<GameItemMapper, GIConfig> factory, IEnumerable<IStructureInitializer> others = null)
		: base(manager, factory, others)
	{
	}
}
