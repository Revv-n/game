using System.Collections.Generic;
using GreenT.HornyScapes.Data;
using StripClub.Model.Shop;
using Zenject;

namespace GreenT.HornyScapes.Bank;

public class SubscriptionLotInitializer : StructureInitializerViaArray<SubscriptionLotMapper, SubscriptionLot, Lot>
{
	public SubscriptionLotInitializer(IManager<Lot> manager, IFactory<SubscriptionLotMapper, SubscriptionLot> factory, IEnumerable<IStructureInitializer> others = null)
		: base(manager, factory, others)
	{
	}
}
