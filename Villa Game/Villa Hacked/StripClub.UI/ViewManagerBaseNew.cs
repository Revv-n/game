using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StripClub.UI;

public abstract class ViewManagerBaseNew<TSource> : ViewCollection<IView<TSource>>, IGenericViewManager<TSource>, IViewManager
{
	public virtual IView<TSource> Display(TSource source)
	{
		IView<TSource> view = views.FirstOrDefault((IView<TSource> _view) => !_view.IsActive());
		if (view == null)
		{
			view = Create();
			views.Add(view);
		}
		view.Set(source);
		view.Display(isOn: true);
		return view;
	}

	public virtual IEnumerable<IView<TSource>> Display(IEnumerable<TSource> sources)
	{
		List<IView<TSource>> result = new List<IView<TSource>>();
		foreach (TSource source in sources)
		{
			Display(source);
		}
		return result;
	}

	protected abstract IView<TSource> Create();
}
public abstract class ViewManagerBaseNew<TSource, TView> : ViewCollection<TView>, IViewManager<TSource, TView>, IViewManager where TView : MonoView<TSource>
{
	public virtual TView Display(TSource source)
	{
		TView val = null;
		try
		{
			val = views.FirstOrDefault((TView _view) => !_view.IsActive());
		}
		catch (MissingReferenceException)
		{
			views.RemoveAll((TView x) => x == null);
		}
		catch (Exception ex2)
		{
			Debug.Log("Exception: " + ex2.GetType());
		}
		if (val == null)
		{
			val = Create();
			val.gameObject.SetActive(value: false);
			views.Add(val);
		}
		val.Set(source);
		val.Display(display: true);
		return val;
	}

	public override void HideAll()
	{
		foreach (TView item in views.Where((TView _view) => _view.IsActive()))
		{
			item.gameObject.SetActive(value: false);
		}
	}

	public virtual void Hide(TSource source)
	{
		GetViewOrDefault(source)?.gameObject.SetActive(value: false);
	}

	public TView GetViewOrDefault(TSource source)
	{
		return views.FirstOrDefault((TView _view) => _view.IsActive() && _view.Source.Equals(source));
	}

	public virtual IEnumerable<TView> Display(IEnumerable<TSource> sources)
	{
		List<TView> result = new List<TView>();
		foreach (TSource source in sources)
		{
			Display(source);
		}
		return result;
	}

	protected abstract TView Create();
}
