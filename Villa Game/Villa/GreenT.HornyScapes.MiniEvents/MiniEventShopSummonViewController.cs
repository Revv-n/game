using System.Collections.Generic;
using System.Linq;
using GreenT.Types;
using StripClub.Model.Shop;
using StripClub.UI;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class MiniEventShopSummonViewController : BaseTabVariantsViewController<Lot, MiniEventShopSummonView>, IShopViewController, IViewController
{
	public IEnumerable<Lot> CurrentLots { get; private set; }

	public MiniEventShopSummonViewController(IManager<Lot> manager, MiniEventShopSummonViewManager viewManager, MiniEventShopSummonSingleViewManager singleViewManager)
		: base(manager, (IViewManager<Lot, MiniEventShopSummonView>)viewManager, (IViewManager<Lot, MiniEventShopSummonView>)singleViewManager)
	{
	}

	protected override IEnumerable<Lot> GetSources(CompositeIdentificator identificator)
	{
		CurrentLots = _manager.Collection.Where((Lot lot) => lot.TabID == identificator[0] && lot.Locker.IsOpen.Value).ToArray();
		return CurrentLots;
	}
}
