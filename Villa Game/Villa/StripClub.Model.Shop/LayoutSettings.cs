namespace StripClub.Model.Shop;

public abstract class LayoutSettings
{
	protected readonly string parameters;

	public LayoutSettings(string parameters)
	{
		this.parameters = parameters;
	}
}
