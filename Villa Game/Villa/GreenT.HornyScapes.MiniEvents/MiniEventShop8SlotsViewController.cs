using System.Collections.Generic;
using System.Linq;
using GreenT.Types;
using StripClub.Model.Shop;
using StripClub.UI;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class MiniEventShop8SlotsViewController : BaseTabVariantsIEnumerableViewController<Lot, MiniEvent8SlotsLotView, MiniEventShop8SlotsRootView, MiniEventShop8SlotsSingleRootView>, IShopViewController, IViewController
{
	public IEnumerable<Lot> CurrentLots { get; private set; }

	public MiniEventShop8SlotsViewController(IManager<Lot> manager, IViewManager<IEnumerable<Lot>, MiniEventShop8SlotsRootView> viewManager, IViewManager<IEnumerable<Lot>, MiniEventShop8SlotsSingleRootView> singleViewManager)
		: base(manager, viewManager, singleViewManager)
	{
	}

	protected override IEnumerable<Lot> GetSources(CompositeIdentificator identificator)
	{
		CurrentLots = _manager.Collection.Where((Lot lot) => lot.TabID == identificator[0] && lot.Locker.IsOpen.Value).ToArray();
		return CurrentLots;
	}
}
