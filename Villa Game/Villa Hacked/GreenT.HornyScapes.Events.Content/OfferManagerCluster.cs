using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Bank;
using GreenT.Types;

namespace GreenT.HornyScapes.Events.Content;

public class OfferManagerCluster : ContentCluster<OfferSettings.Manager>
{
	public void Initialize()
	{
		using Enumerator enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			enumerator.Current.Value.Initialize();
		}
	}

	public IEnumerable<OfferSettings> GetOffersForTypes(IEnumerable<ContentType> types)
	{
		IEnumerable<OfferSettings> enumerable = new List<OfferSettings>();
		foreach (ContentType type in types)
		{
			enumerable = enumerable.Concat(base[type].Collection);
		}
		return enumerable;
	}
}
