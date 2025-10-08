using System.Collections.Generic;
using System.Linq;
using StripClub.Model.Shop;
using StripClub.UI.Shop;

namespace GreenT.HornyScapes.Bank.UI;

public class RechargeSectionView : BankSectionView
{
	protected override IEnumerable<Lot> VisibleLots(IEnumerable<Lot> targetLots)
	{
		return (from _lot in targetLots
			where _lot.Received < _lot.AvailableCount
			orderby _lot.SerialNumber
			select _lot).Take(1).ToList();
	}
}
