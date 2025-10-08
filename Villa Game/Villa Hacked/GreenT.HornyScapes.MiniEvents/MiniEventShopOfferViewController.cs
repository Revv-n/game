using System.Collections.Generic;
using System.Linq;
using GreenT.Types;
using StripClub.Model.Shop;
using StripClub.UI;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class MiniEventShopOfferViewController : BaseViewController<Lot, MiniEventShopOfferView>, IShopViewController, IViewController
{
	public IEnumerable<Lot> CurrentLots { get; private set; }

	public MiniEventShopOfferViewController(IManager<Lot> manager, IViewManager<Lot, MiniEventShopOfferView> viewManager)
		: base(manager, viewManager)
	{
	}

	protected override IEnumerable<Lot> GetSources(CompositeIdentificator identificator)
	{
		CurrentLots = _manager.Collection.Where((Lot lot) => lot.TabID == identificator[0]);
		return CurrentLots.Where((Lot lot) => lot.Locker.IsOpen.Value);
	}
}
