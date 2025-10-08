using System.Collections.Generic;
using System.Linq;
using StripClub.Model.Shop;
using StripClub.UI.Shop;

namespace GreenT.HornyScapes.Bank.Offer.UI;

public class SingleSubscriptionOfferSectionView : SubscriptionOfferSectionView
{
	protected override void DisplayAvailableLots(IEnumerable<Lot> lots)
	{
		viewManager.HideAll();
		if (lots.FirstOrDefault() != null)
		{
			LotContainer source = new LotContainer(lots);
			viewManager.Display(source);
			SetGoToInfo();
		}
	}
}
