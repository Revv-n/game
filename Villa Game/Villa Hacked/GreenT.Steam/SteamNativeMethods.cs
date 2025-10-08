using System.Text;
using AOT;
using Steamworks;
using UnityEngine;

namespace GreenT.Steam;

public class SteamNativeMethods
{
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	private static void InitOnPlayMode()
	{
		SteamState.SetEverInitialized(state: false);
	}

	[MonoPInvokeCallback(typeof(SteamAPIWarningMessageHook_t))]
	public void SteamAPIDebugTextHook(int nSeverity, StringBuilder pchDebugText)
	{
		Debug.LogWarning(pchDebugText);
	}
}
