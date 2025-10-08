using StripClub.Model.Shop;
using StripClub.UI;
using Zenject;

namespace GreenT.HornyScapes.MiniEvents;

public class MiniEventShopSummonViewManager : CustomViewManager<Lot, MiniEventShopSummonView>
{
	[Inject]
	private void Init(MiniEventSummonLotViewFactory viewFactory)
	{
		base.viewFactory = viewFactory;
	}
}
