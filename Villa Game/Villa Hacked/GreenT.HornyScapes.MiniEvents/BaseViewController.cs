using System.Collections.Generic;
using System.Linq;
using GreenT.Types;
using StripClub.UI;

namespace GreenT.HornyScapes.MiniEvents;

public abstract class BaseViewController<TSource, TView> : IViewController, IContentable where TView : IView<TSource>
{
	protected IManager<TSource> _manager;

	protected IViewManager<TSource, TView> _viewManager;

	protected BaseViewController(IManager<TSource> manager, IViewManager<TSource, TView> viewManager)
	{
		_manager = manager;
		_viewManager = viewManager;
	}

	public virtual void Show(CompositeIdentificator identificator, bool isMultiTabbed = false)
	{
		Display(GetSources(identificator));
	}

	public bool HasAnyContentAvailable(CompositeIdentificator identificator)
	{
		return GetSources(identificator).Any();
	}

	public virtual void HideAll()
	{
		_viewManager.HideAll();
	}

	protected abstract IEnumerable<TSource> GetSources(CompositeIdentificator identificator);

	protected virtual void Display(IEnumerable<TSource> sources)
	{
		foreach (TSource source in sources)
		{
			_viewManager.Display(source);
		}
	}

	protected virtual void OrderViews()
	{
		foreach (TView orderedView in GetOrderedViews())
		{
			orderedView.Display(isOn: true);
		}
	}

	protected virtual IEnumerable<TView> GetOrderedViews()
	{
		return _viewManager.VisibleViews;
	}
}
