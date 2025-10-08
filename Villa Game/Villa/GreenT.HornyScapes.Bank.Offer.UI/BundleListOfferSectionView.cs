using System.Collections.Generic;
using StripClub.Model.Shop;
using Zenject;

namespace GreenT.HornyScapes.Bank.Offer.UI;

public class BundleListOfferSectionView : OfferSectionView
{
	protected IClock Clock { get; private set; }

	[Inject]
	private void Init(IClock clock)
	{
		Clock = clock;
	}

	protected override IEnumerable<Lot> VisibleLots(IEnumerable<Lot> targetLots)
	{
		return base.VisibleLots(targetLots);
	}
}
