using System.Collections.Generic;
using System.Linq;
using StripClub.Model.Shop;
using StripClub.UI.Shop;

namespace GreenT.HornyScapes.Bank.UI;

public class ShopBundlesSectionView : BankSectionView
{
	private void OnEnable()
	{
		Init();
	}

	public void Init()
	{
		IEnumerable<BundleLot> enumerable = from _lot in _lotManager.Collection.OfType<BundleLot>()
			where _lot.Locker.IsOpen.Value
			select _lot;
		viewManager.HideAll();
		foreach (BundleLot item in enumerable)
		{
			LotContainer source = new LotContainer(new Lot[1] { item });
			viewManager.Display(source);
		}
	}
}
