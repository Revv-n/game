using System.Collections.Generic;
using GreenT.HornyScapes.Data;
using GreenT.Types;
using Zenject;

namespace GreenT.HornyScapes.Bank.Data;

public class OfferConfigInitializer : LimitedContentConfigStructureInitializer<OfferMapper, OfferSettings, OfferSettings.Manager>
{
	public OfferConfigInitializer(IDictionary<ContentType, OfferSettings.Manager> dictionary, IFactory<OfferMapper, OfferSettings> factory, IEnumerable<IStructureInitializer> others = null)
		: base(dictionary, factory, others)
	{
	}
}
