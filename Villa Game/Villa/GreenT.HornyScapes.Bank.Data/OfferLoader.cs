using System.Collections.Generic;
using GreenT.Data;
using GreenT.Types;
using StripClub.Model.Data;
using Zenject;

namespace GreenT.HornyScapes.Bank.Data;

public class OfferLoader : LimitedContentLoader<OfferMapper, OfferSettings, OfferSettings.Manager>
{
	public OfferLoader(ILoader<IEnumerable<OfferMapper>> loader, IFactory<OfferMapper, OfferSettings> factory, IDictionary<ContentType, OfferSettings.Manager> cluster)
		: base(loader, factory, cluster)
	{
	}
}
