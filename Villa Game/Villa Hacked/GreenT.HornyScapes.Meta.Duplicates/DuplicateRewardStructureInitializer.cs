using System.Collections.Generic;
using GreenT.HornyScapes.Data;
using Zenject;

namespace GreenT.HornyScapes.Meta.Duplicates;

public class DuplicateRewardStructureInitializer : StructureInitializerViaArray<DuplicateRewardMapper, DuplicateReward>
{
	public DuplicateRewardStructureInitializer(IManager<DuplicateReward> manager, IFactory<DuplicateRewardMapper, DuplicateReward> factory, IEnumerable<IStructureInitializer> others = null)
		: base(manager, factory, others)
	{
	}
}
