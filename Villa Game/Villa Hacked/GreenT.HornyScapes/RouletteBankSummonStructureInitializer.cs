using System.Collections.Generic;
using GreenT.HornyScapes.Data;
using Zenject;

namespace GreenT.HornyScapes;

public sealed class RouletteBankSummonStructureInitializer : StructureInitializerViaArray<RouletteBankSummonMapper, RouletteBankSummonLot>
{
	public RouletteBankSummonStructureInitializer(IManager<RouletteBankSummonLot> manager, IFactory<RouletteBankSummonMapper, RouletteBankSummonLot> factory, IEnumerable<IStructureInitializer> others)
		: base(manager, factory, others)
	{
	}
}
