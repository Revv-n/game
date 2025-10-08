using StripClub.UI;
using UnityEngine;

namespace GreenT.HornyScapes.MiniEvents;

public class InteractableTabsViewManager<TSource, TView> : ViewManager<TSource, TView> where TSource : IIdentifiable where TView : TabView<TSource>, IView<TSource>
{
	[SerializeField]
	private MiniEventWindowView _windowView;

	protected override TView Create(TSource source)
	{
		TView val = base.Create(source);
		val.Init(_windowView);
		return val;
	}
}
