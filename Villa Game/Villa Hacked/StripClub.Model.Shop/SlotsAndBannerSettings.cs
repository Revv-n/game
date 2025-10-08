namespace StripClub.Model.Shop;

public class SlotsAndBannerSettings : LayoutSettings
{
	public string SettingsPath => parameters;

	public SlotsAndBannerSettings(string parameters)
		: base(parameters)
	{
	}
}
