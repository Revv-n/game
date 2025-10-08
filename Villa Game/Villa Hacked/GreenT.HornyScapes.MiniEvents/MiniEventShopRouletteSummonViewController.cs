using System.Collections.Generic;
using System.Linq;
using GreenT.Types;
using StripClub.UI;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class MiniEventShopRouletteSummonViewController : BaseTabVariantsViewController<RouletteSummonLot, MiniEventShopRouletteSummonView>
{
	public MiniEventShopRouletteSummonViewController(IManager<RouletteSummonLot> manager, MiniEventShopRouletteSummonViewManager viewManager, MiniEventShopRouletteSingleViewManager singleViewManager)
		: base(manager, (IViewManager<RouletteSummonLot, MiniEventShopRouletteSummonView>)viewManager, (IViewManager<RouletteSummonLot, MiniEventShopRouletteSummonView>)singleViewManager)
	{
	}

	protected override IEnumerable<RouletteSummonLot> GetSources(CompositeIdentificator identificator)
	{
		return _manager.Collection.Where((RouletteSummonLot lot) => lot.ID == identificator[0] && lot.Locker.IsOpen.Value).ToArray();
	}
}
