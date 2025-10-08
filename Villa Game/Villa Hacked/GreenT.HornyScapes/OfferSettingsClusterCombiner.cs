using System.Collections.Generic;
using GreenT.HornyScapes.Bank;
using GreenT.Types;

namespace GreenT.HornyScapes;

public sealed class OfferSettingsClusterCombiner : ClusterCombiner<OfferSettings, OfferSettings.Manager>
{
	public OfferSettingsClusterCombiner(IDictionary<ContentType, OfferSettings.Manager> cluster)
		: base(cluster)
	{
	}
}
