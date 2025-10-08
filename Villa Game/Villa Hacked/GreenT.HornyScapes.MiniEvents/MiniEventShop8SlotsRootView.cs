using System.Collections.Generic;
using StripClub.Model.Shop;
using StripClub.UI;
using Zenject;

namespace GreenT.HornyScapes.MiniEvents;

public class MiniEventShop8SlotsRootView : MonoView<IEnumerable<Lot>>
{
	private MiniEventShop8SlotsViewManager _miniEventShop8SlotsViewManager;

	[Inject]
	private void Init(MiniEventShop8SlotsViewManager miniEventShop8SlotsViewManager)
	{
		_miniEventShop8SlotsViewManager = miniEventShop8SlotsViewManager;
	}

	public override void Set(IEnumerable<Lot> sources)
	{
		base.Set(sources);
		_miniEventShop8SlotsViewManager.HideAll();
		foreach (Lot source in sources)
		{
			_miniEventShop8SlotsViewManager.Display(source);
		}
	}
}
