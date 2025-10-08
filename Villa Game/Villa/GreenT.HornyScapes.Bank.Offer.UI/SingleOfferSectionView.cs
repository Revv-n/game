using System.Collections.Generic;
using System.Linq;
using StripClub.Model.Shop;
using StripClub.UI.Shop;

namespace GreenT.HornyScapes.Bank.Offer.UI;

public class SingleOfferSectionView : OfferSectionView
{
	protected override void DisplayAvailableLots(IEnumerable<Lot> lots)
	{
		viewManager.HideAll();
		if (lots.FirstOrDefault() != null)
		{
			LotContainer source = new LotContainer(lots);
			viewManager.Display(source);
		}
	}

	protected override void DisplayCloneLots(IEnumerable<Lot> lots)
	{
		viewManager.HideAll();
		Lot lot = lots.FirstOrDefault();
		if (lot == null)
		{
			return;
		}
		LotContainer source;
		if (lot is BundleLot)
		{
			List<Lot> list = new List<Lot>();
			foreach (Lot lot2 in lots)
			{
				list.Add((lot2 as BundleLot).CloneWithAccessibleLocker());
			}
			source = new LotContainer(list);
		}
		else
		{
			source = new LotContainer(lots);
		}
		viewManager.Display(source);
		if (lots.Skip(1).Any())
		{
			IEnumerable<int> collection = from _lot in lots.Skip(1)
				select _lot.ID;
			Console.SendLogCollection("Не будут отображены лоты с id:", collection, LogType.Warning);
		}
	}
}
