using System.Collections.Generic;

namespace StripClub.UI;

public interface IViewManager
{
	void HideAll();
}
public interface IViewManager<out T> : IViewManager where T : IView
{
	T GetView();
}
public interface IViewManager<in TSource, out KView> : IViewManager where KView : IView<TSource>
{
	IEnumerable<KView> VisibleViews { get; }

	KView Display(TSource source);
}
