using System.Collections.Generic;
using GreenT.Types;
using StripClub.UI;

namespace GreenT.HornyScapes.MiniEvents;

public abstract class BaseTabVariantsViewController<TSource, TView> : BaseViewController<TSource, TView> where TView : IView<TSource>
{
	protected IViewManager<TSource, TView> _singleViewManager;

	protected BaseTabVariantsViewController(IManager<TSource> manager, IViewManager<TSource, TView> viewManager, IViewManager<TSource, TView> singleViewManager)
		: base(manager, viewManager)
	{
		_singleViewManager = singleViewManager;
	}

	public override void Show(CompositeIdentificator identificator, bool isMultiTabbed)
	{
		Display(GetSources(identificator), isMultiTabbed);
	}

	public override void HideAll()
	{
		base.HideAll();
		_singleViewManager.HideAll();
	}

	protected void Display(IEnumerable<TSource> sources, bool isMultiTabbed)
	{
		IViewManager<TSource, TView> viewManager = ((!isMultiTabbed) ? _singleViewManager : _viewManager);
		foreach (TSource source in sources)
		{
			viewManager.Display(source);
		}
	}
}
