using Steamworks;

namespace GreenT.Steam;

public class SteamWarningMessageHandler
{
	private readonly SteamNativeMethods _steamNativeMethods;

	public SteamWarningMessageHandler(SteamNativeMethods steamNativeMethods)
	{
		_steamNativeMethods = steamNativeMethods;
	}

	public void SetWarningMessageHook()
	{
		if (SteamState.SteamAPIWarningMessageHook == null)
		{
			SteamState.SteamAPIWarningMessageHook = _steamNativeMethods.SteamAPIDebugTextHook;
			SteamClient.SetWarningMessageHook(SteamState.SteamAPIWarningMessageHook);
		}
	}
}
