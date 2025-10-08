using System.Collections.Generic;
using System.Linq;
using StripClub.UI;

namespace GreenT.HornyScapes.MiniEvents;

public abstract class BaseIEnumerableViewController<TSource, TView, TIEnumerableView> : BaseViewController<TSource, TView> where TView : IView<TSource> where TIEnumerableView : IView<IEnumerable<TSource>>
{
	protected new IViewManager<IEnumerable<TSource>, TIEnumerableView> _viewManager;

	protected BaseIEnumerableViewController(IManager<TSource> manager, IViewManager<IEnumerable<TSource>, TIEnumerableView> viewManager)
		: base(manager, (IViewManager<TSource, TView>)null)
	{
		_viewManager = viewManager;
	}

	public override void HideAll()
	{
		_viewManager.HideAll();
	}

	protected override void Display(IEnumerable<TSource> sources)
	{
		if (sources.Any())
		{
			_viewManager.Display(sources);
		}
	}
}
