using System.Collections.Generic;
using System.Linq;

namespace StripClub.UI;

public abstract class ViewManagerBase<TSource> : ViewCollection<TSource>, IViewManager<TSource>, IViewManager where TSource : IView
{
	public virtual TSource GetView()
	{
		TSource val = views.FirstOrDefault((TSource _view) => !_view.IsActive());
		if (val == null)
		{
			val = Create();
			views.Add(val);
		}
		else
		{
			val.Display(isOn: true);
		}
		return val;
	}

	protected abstract TSource Create();
}
public abstract class ViewManagerBase<TSource, TView> : ViewCollection<TView>, IViewManager<TSource, TView>, IViewManager where TView : IView<TSource>
{
	public virtual TView Display(TSource source)
	{
		TView val = views.FirstOrDefault((TView _view) => CheckAvailableView(_view, source));
		if (val == null)
		{
			val = Create(source);
			views.Add(val);
		}
		else
		{
			val.Set(source);
			val.Display(isOn: true);
		}
		return val;
	}

	protected virtual bool CheckAvailableView(TView view, TSource source)
	{
		return !view.IsActive();
	}

	public virtual IEnumerable<TView> Display(IEnumerable<TSource> sources)
	{
		List<TView> list = new List<TView>();
		foreach (TSource source in sources)
		{
			TView item = Display(source);
			list.Add(item);
		}
		return list;
	}

	protected abstract TView Create(TSource source);
}
