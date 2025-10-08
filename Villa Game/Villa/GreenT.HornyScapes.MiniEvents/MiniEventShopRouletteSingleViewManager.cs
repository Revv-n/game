using StripClub.UI;
using Zenject;

namespace GreenT.HornyScapes.MiniEvents;

public class MiniEventShopRouletteSingleViewManager : CustomViewManager<RouletteSummonLot, MiniEventShopRouletteSummonView>
{
	[Inject]
	private void Init(MiniEventSingleRouletteLotViewFactory viewFactory)
	{
		base.viewFactory = viewFactory;
	}
}
