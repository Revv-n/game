using System.Collections.Generic;
using System.Linq;
using GreenT.Types;
using StripClub.Model.Shop;
using StripClub.UI;
using StripClub.UI.Shop;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class MiniEventShopBundlesViewController : BaseTabVariantsIEnumerableViewController<Lot, SimpleBundleLotView, MiniEventShopBundlesRootView, MiniEventShopBundlesSingleRootView>, IShopViewController, IViewController
{
	public IEnumerable<Lot> CurrentLots { get; private set; }

	public MiniEventShopBundlesViewController(IManager<Lot> manager, IViewManager<IEnumerable<Lot>, MiniEventShopBundlesRootView> viewManager, IViewManager<IEnumerable<Lot>, MiniEventShopBundlesSingleRootView> singleViewManager)
		: base(manager, viewManager, singleViewManager)
	{
	}

	protected override IEnumerable<Lot> GetSources(CompositeIdentificator identificator)
	{
		CurrentLots = _manager.Collection.Where((Lot lot) => lot.TabID == identificator[0] && lot.Locker.IsOpen.Value).ToArray();
		return CurrentLots;
	}
}
