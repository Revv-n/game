namespace GreenT.HornyScapes;

public static class PlatformHelper
{
	public static bool IsEditorPlatform()
	{
		return false;
	}

	public static bool IsEpochaMonetization()
	{
		return false;
	}

	public static bool IsNutakuMonetization()
	{
		return false;
	}

	public static bool IsSteamMonetization()
	{
		return true;
	}

	public static bool IsHaremMonetization()
	{
		return false;
	}

	public static string GetPlatformName()
	{
		return "Steam";
	}

	public static bool IsErolabsMonetization()
	{
		return false;
	}
}
