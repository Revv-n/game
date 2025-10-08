using System.Collections.Generic;
using GreenT.HornyScapes.Data;
using StripClub.Model.Shop;
using StripClub.Model.Shop.Data;
using Zenject;

namespace GreenT.HornyScapes.Bank.Data;

public class SummonLotInitializer : StructureInitializerViaArray<SummonMapper, SummonLot, Lot>
{
	public SummonLotInitializer(IManager<Lot> manager, IFactory<SummonMapper, SummonLot> factory, IEnumerable<IStructureInitializer> others = null)
		: base(manager, factory, others)
	{
	}
}
