using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Bank.Offer.UI;
using StripClub.Model.Shop;
using StripClub.UI.Shop;

namespace GreenT.HornyScapes._HornyScapes._Scripts.Bank.Offers.UI;

public class DoubleOfferSectionView : OfferSectionView
{
	protected override void DisplayAvailableLots(IEnumerable<Lot> lots)
	{
		viewManager.HideAll();
		LotContainer source = new LotContainer(lots);
		viewManager.Display(source);
	}

	protected override void DisplayCloneLots(IEnumerable<Lot> lots)
	{
		viewManager.HideAll();
		LotContainer source;
		if (lots.First() is BundleLot)
		{
			List<Lot> list = new List<Lot>();
			foreach (Lot lot in lots)
			{
				list.Add((lot as BundleLot).CloneWithAccessibleLocker());
			}
			source = new LotContainer(list);
		}
		else
		{
			source = new LotContainer(lots);
		}
		viewManager.Display(source);
		if (lots.Skip(2).Any())
		{
			IEnumerable<int> collection = from _lot in lots.Skip(1)
				select _lot.ID;
			Console.SendLogCollection("Не будут отображены лоты с id:", collection, LogType.Warning);
		}
	}
}
