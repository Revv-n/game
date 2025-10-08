using System.Collections.Generic;
using GreenT.HornyScapes.Data;
using Zenject;

namespace GreenT.HornyScapes;

public sealed class RatingStructureInitializer : StructureInitializerViaArray<RatingMapper, Rating>
{
	public RatingStructureInitializer(IManager<Rating> manager, IFactory<RatingMapper, Rating> factory, IEnumerable<IStructureInitializer> others = null)
		: base(manager, factory, others)
	{
	}
}
