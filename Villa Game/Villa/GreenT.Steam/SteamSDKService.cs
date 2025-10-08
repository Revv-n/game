using System;
using GreenT.HornyScapes.Exceptions;
using Steamworks;
using UnityEngine;

namespace GreenT.Steam;

public sealed class SteamSDKService : IDisposable
{
	private readonly IExceptionHandler _exceptionHandler;

	public SteamSDKService(IExceptionHandler exceptionHandler)
	{
		_exceptionHandler = exceptionHandler;
	}

	public void Initialize()
	{
		if (SteamState.EverInitialized)
		{
			HandleException();
			return;
		}
		CheckErrors();
		if (!RestartAppIfNeed() && InitSteamAPI())
		{
			SteamUserStats.RequestCurrentStats();
			SteamState.SetEverInitialized(state: true);
		}
	}

	private bool InitSteamAPI()
	{
		bool num = SteamAPI.Init();
		SteamState.SetInitialized(num);
		if (!num)
		{
			_exceptionHandler.Handle("[Steamworks.NET] SteamAPI_Init() failed. Refer to Valve's documentation or the comment above this line for more information.");
			return false;
		}
		CSteamID steamID = SteamUser.GetSteamID();
		Debug.Log($"steamID: {steamID}");
		return true;
	}

	private bool RestartAppIfNeed()
	{
		try
		{
			if (SteamAPI.RestartAppIfNecessary(AppId_t.Invalid))
			{
				Application.Quit();
				return true;
			}
		}
		catch (DllNotFoundException innerEx)
		{
			string reason = "[Steamworks.NET] Could not load [lib]steam_api.dll/so/dylib. It's likely not in the correct location. Refer to the README for more details.\n";
			_exceptionHandler.Handle(innerEx, reason);
			Application.Quit();
			return true;
		}
		return false;
	}

	private void CheckErrors()
	{
		if (!Packsize.Test())
		{
			_exceptionHandler.Handle("[Steamworks.NET] Packsize Test returned false, the wrong version of Steamworks.NET is being run in this platform.");
		}
		if (!DllCheck.Test())
		{
			_exceptionHandler.Handle("[Steamworks.NET] DllCheck Test returned false, One or more of the Steamworks binaries seems to be the wrong version.");
		}
	}

	public void Dispose()
	{
		SteamAPI.Shutdown();
	}

	private void HandleException()
	{
		_exceptionHandler.Handle("Tried to Initialize the SteamAPI twice in one session!");
	}
}
