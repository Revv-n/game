using System.Collections.Generic;
using GreenT.HornyScapes.Data;
using Zenject;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class RouletteSummonStructureInitializer : StructureInitializerViaArray<RouletteSummonMapper, RouletteSummonLot>
{
	public RouletteSummonStructureInitializer(IManager<RouletteSummonLot> manager, IFactory<RouletteSummonMapper, RouletteSummonLot> factory, IEnumerable<IStructureInitializer> others)
		: base(manager, factory, others)
	{
	}
}
