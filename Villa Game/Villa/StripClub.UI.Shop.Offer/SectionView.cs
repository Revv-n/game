using System.Collections.Generic;
using System.Linq;
using StripClub.Model.Shop;

namespace StripClub.UI.Shop.Offer;

public class SectionView : BankSectionView
{
	protected override IEnumerable<Lot> VisibleLots(IEnumerable<Lot> targetLots)
	{
		return (from _lot in targetLots
			where _lot.IsAvailable()
			orderby _lot.SerialNumber
			select _lot).Take(1).ToList();
	}
}
