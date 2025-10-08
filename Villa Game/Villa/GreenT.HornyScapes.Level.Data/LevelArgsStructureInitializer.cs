using System.Collections.Generic;
using GreenT.HornyScapes.Data;
using Zenject;

namespace GreenT.HornyScapes.Level.Data;

public class LevelArgsStructureInitializer : StructureInitializerViaArray<LevelsArgsMapper, LevelsArgs>
{
	public LevelArgsStructureInitializer(IManager<LevelsArgs> manager, IFactory<LevelsArgsMapper, LevelsArgs> factory, IEnumerable<IStructureInitializer> others = null)
		: base(manager, factory, others)
	{
	}
}
