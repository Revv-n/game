namespace StripClub.UI;

public interface IGenericViewManager<in TSource> : IViewManager
{
	IView<TSource> Display(TSource source);
}
