using System.Collections.Generic;
using GreenT.Types;
using StripClub.UI;

namespace GreenT.HornyScapes.MiniEvents;

public abstract class BaseTabVariantsIEnumerableViewController<TSource, TView, TIEnumerableView, TIEnumerableSingleView> : BaseIEnumerableViewController<TSource, TView, TIEnumerableView> where TView : IView<TSource> where TIEnumerableView : IView<IEnumerable<TSource>> where TIEnumerableSingleView : IView<IEnumerable<TSource>>
{
	protected IViewManager<IEnumerable<TSource>, TIEnumerableSingleView> _singleViewManager;

	protected BaseTabVariantsIEnumerableViewController(IManager<TSource> manager, IViewManager<IEnumerable<TSource>, TIEnumerableView> viewManager, IViewManager<IEnumerable<TSource>, TIEnumerableSingleView> singleViewManager)
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
		if (isMultiTabbed)
		{
			_viewManager.Display(sources);
		}
		else
		{
			_singleViewManager.Display(sources);
		}
	}
}
