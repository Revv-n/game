using System.Collections.Generic;
using StripClub.Model.Shop;
using StripClub.UI.Shop;

namespace GreenT.HornyScapes.Bank.Offer.UI;

public class DoubleSubscriptionOfferSectionView : SubscriptionOfferSectionView
{
	protected override void DisplayAvailableLots(IEnumerable<Lot> lots)
	{
		viewManager.HideAll();
		foreach (Lot lot in lots)
		{
			LotContainer source = new LotContainer(new Lot[1] { lot });
			viewManager.Display(source);
		}
		SetGoToInfo();
	}
}
