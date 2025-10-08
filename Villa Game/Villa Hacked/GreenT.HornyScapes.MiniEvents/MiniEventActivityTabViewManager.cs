using UnityEngine;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class MiniEventActivityTabViewManager : InteractableTabsViewManager<MiniEventActivityTab, MiniEventActivityTabView>
{
	[SerializeField]
	private CustomGridLayoutAdjuster _customGridLayoutAdjuster;

	protected override MiniEventActivityTabView Create(MiniEventActivityTab source)
	{
		MiniEventActivityTabView miniEventActivityTabView = base.Create(source);
		miniEventActivityTabView.CustomGridLayoutAdjuster = _customGridLayoutAdjuster;
		miniEventActivityTabView.Display(display: true);
		return miniEventActivityTabView;
	}
}
