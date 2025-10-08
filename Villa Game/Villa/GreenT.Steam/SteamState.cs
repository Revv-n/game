using Steamworks;

namespace GreenT.Steam;

public static class SteamState
{
	public const string IDENTITY = "unityauthenticationservice";

	public static SteamAPIWarningMessageHook_t SteamAPIWarningMessageHook;

	public static bool EverInitialized { get; private set; }

	public static bool Initialized { get; private set; }

	public static void SetEverInitialized(bool state)
	{
		EverInitialized = state;
	}

	public static void SetInitialized(bool state)
	{
		Initialized = state;
	}
}
