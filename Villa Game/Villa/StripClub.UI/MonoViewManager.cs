using Zenject;

namespace StripClub.UI;

public class MonoViewManager<TSource, TView> : ViewManagerBaseNew<TSource, TView> where TView : MonoView<TSource>
{
	protected IFactory<TView> itemViewFactory;

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
public class MonoViewManager<TSource> : ViewManagerBaseNew<TSource>
{
	protected IFactory<IView<TSource>> itemViewFactory;

	[Inject]
	public void Init(IFactory<IView<TSource>> itemViewFactory)
	{
		this.itemViewFactory = itemViewFactory;
	}

	protected override IView<TSource> Create()
	{
		return itemViewFactory.Create();
	}
}
