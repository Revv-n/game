using System.Collections.Generic;
using System.Linq;
using GreenT.Types;
using StripClub.Model.Shop;
using StripClub.UI;
using StripClub.UI.Shop.Offer;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class MiniEventShopDoubleOfferViewController : BaseIEnumerableViewController<Lot, BundleLotView, MiniEventShopDoubleOfferView>, IShopViewController, IViewController
{
	private const int MIN_LOTS_AMOUNT = 2;

	public IEnumerable<Lot> CurrentLots { get; private set; }

	public MiniEventShopDoubleOfferViewController(IManager<Lot> manager, IViewManager<IEnumerable<Lot>, MiniEventShopDoubleOfferView> viewManager)
		: base(manager, viewManager)
	{
	}

	protected override IEnumerable<Lot> GetSources(CompositeIdentificator identificator)
	{
		CurrentLots = _manager.Collection.Where((Lot lot) => lot.TabID == identificator[0]);
		IEnumerable<Lot> enumerable = CurrentLots.Where((Lot lot) => lot.Locker.IsOpen.Value);
		Lot[] result = new Lot[0];
		if (enumerable.Count() < 2)
		{
			return result;
		}
		return enumerable;
	}
}
