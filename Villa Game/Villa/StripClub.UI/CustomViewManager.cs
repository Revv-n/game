using Zenject;

namespace StripClub.UI;

public class CustomViewManager<TSource, TView> : ViewManagerBase<TSource, TView> where TView : IView<TSource>
{
	protected IFactory<TSource, TView> viewFactory;

	protected override TView Create(TSource source)
	{
		return viewFactory.Create(source);
	}
}
