using StripClub.UI;
using Zenject;

namespace GreenT.HornyScapes.MiniEvents;

public class MiniEventShopRouletteSummonViewManager : CustomViewManager<RouletteSummonLot, MiniEventShopRouletteSummonView>
{
	[Inject]
	private void Init(MiniEventRouletteSummonLotViewFactory viewFactory)
	{
		base.viewFactory = viewFactory;
	}
}
