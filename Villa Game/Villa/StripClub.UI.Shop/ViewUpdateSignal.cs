namespace StripClub.UI.Shop;

public class ViewUpdateSignal
{
	public IView View { get; }

	public ViewUpdateSignal(IView view)
	{
		View = view;
	}
}
