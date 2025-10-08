using System.Collections.Generic;
using System.Linq;
using GreenT.Types;
using StripClub.Model.Shop;
using StripClub.UI;
using StripClub.UI.Shop;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class MiniEventShopChainBundlesViewController : BaseIEnumerableViewController<Lot, ChainBundleLotView, MiniEventShopChainBundlesSingleRootView>, IShopViewController, IViewController
{
	public IEnumerable<Lot> CurrentLots { get; private set; }

	public MiniEventShopChainBundlesViewController(IManager<Lot> manager, IViewManager<IEnumerable<Lot>, MiniEventShopChainBundlesSingleRootView> viewManager)
		: base(manager, viewManager)
	{
	}

	protected override IEnumerable<Lot> GetSources(CompositeIdentificator identificator)
	{
		CurrentLots = _manager.Collection.Where((Lot lot) => lot.TabID == identificator[0] && lot.IsAvailable());
		return CurrentLots;
	}
}
