using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.MiniEvents;
using StripClub.Model;
using Zenject;

namespace StripClub.UI.Shop;

public sealed class MultiContentView : MonoView
{
	private RoulettePreviewSmallCardManager _roulettePreviewSmallCardManager;

	private RoulettePreviewBigCardManager _roulettePreviewBigCardManager;

	[Inject]
	private void Init(RoulettePreviewSmallCardManager roulettePreviewSmallCardManager, RoulettePreviewBigCardManager roulettePreviewBigCardManager)
	{
		_roulettePreviewSmallCardManager = roulettePreviewSmallCardManager;
		_roulettePreviewBigCardManager = roulettePreviewBigCardManager;
	}

	public void Set(IEnumerable<LinkedContent> mainContent, IEnumerable<LinkedContent> secondaryContent)
	{
		if (!mainContent.Any() || mainContent == null || secondaryContent == null)
		{
			return;
		}
		_roulettePreviewBigCardManager.HideAll();
		_roulettePreviewSmallCardManager.HideAll();
		foreach (LinkedContent item in mainContent)
		{
			_roulettePreviewBigCardManager.Display(item);
		}
		foreach (LinkedContent item2 in secondaryContent)
		{
			_roulettePreviewSmallCardManager.Display(item2);
		}
	}
}
