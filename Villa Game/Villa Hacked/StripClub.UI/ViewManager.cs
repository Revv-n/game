using Zenject;

namespace StripClub.UI;

public class ViewManager<TView> : ViewManagerBase<TView> where TView : IView
{
	private IFactory<TView> itemViewFactory;

	[Inject]
	public void Init(IFactory<TView> itemViewFactory)
	{
		this.itemViewFactory = itemViewFactory;
	}

	protected override TView Create()
	{
		return itemViewFactory.Create();
	}
}
public class ViewManager<TSource, TView> : ViewManagerBase<TSource, TView> where TView : IView<TSource>
{
	protected IFactory<TSource, TView> viewFactory;

	[Inject]
	public void Init(IFactory<TSource, TView> viewFactory)
	{
		this.viewFactory = viewFactory;
	}

	protected override TView Create(TSource source)
	{
		return viewFactory.Create(source);
	}
}
