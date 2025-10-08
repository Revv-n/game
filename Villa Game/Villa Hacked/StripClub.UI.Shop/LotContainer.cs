using System.Collections.Generic;
using StripClub.Model.Shop;

namespace StripClub.UI.Shop;

public class LotContainer
{
	public IEnumerable<Lot> Lots;

	public LotContainer(IEnumerable<Lot> Lots)
	{
		this.Lots = Lots;
	}
}
