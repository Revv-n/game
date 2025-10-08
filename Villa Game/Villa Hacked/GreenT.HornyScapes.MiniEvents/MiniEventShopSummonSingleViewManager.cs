using StripClub.Model.Shop;
using StripClub.UI;
using Zenject;

namespace GreenT.HornyScapes.MiniEvents;

public class MiniEventShopSummonSingleViewManager : CustomViewManager<Lot, MiniEventShopSummonView>
{
	[Inject]
	private void Init(MiniEventSingleSummonLotViewFactory viewFactory)
	{
		base.viewFactory = viewFactory;
	}
}
