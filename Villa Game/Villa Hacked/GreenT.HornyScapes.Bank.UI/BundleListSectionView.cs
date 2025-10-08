using System.Collections.Generic;
using StripClub.Model.Shop;
using StripClub.UI.Shop;
using Zenject;

namespace GreenT.HornyScapes.Bank.UI;

public class BundleListSectionView : BankSectionView
{
	private IClock _clock;

	[Inject]
	private void Init(IClock clock)
	{
		_clock = clock;
	}

	protected override IEnumerable<Lot> VisibleLots(IEnumerable<Lot> lots)
	{
		return base.VisibleLots(lots);
	}
}
