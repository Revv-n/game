using System.Collections.Generic;
using StripClub.Model.Shop;
using StripClub.UI;
using Zenject;

namespace GreenT.HornyScapes.MiniEvents;

public class MiniEventShopBundlesRootView : MonoView<IEnumerable<Lot>>
{
	private MiniEventShopBundleViewManager _miniEventShopBundleViewManager;

	[Inject]
	private void Init(MiniEventShopBundleViewManager miniEventShopBundleViewManager)
	{
		_miniEventShopBundleViewManager = miniEventShopBundleViewManager;
	}

	public override void Set(IEnumerable<Lot> sources)
	{
		base.Set(sources);
		_miniEventShopBundleViewManager.HideAll();
		foreach (Lot source in sources)
		{
			_miniEventShopBundleViewManager.Display(source);
		}
	}
}
