using System;
using Steamworks;
using UnityEngine;

namespace GreenT.Steam;

public static class SteamConstants
{
	public static string GetSteamAppID()
	{
		try
		{
			return SteamUtils.GetAppID().ToString();
		}
		catch (Exception ex)
		{
			Debug.LogError($"Error on try to get SteamAppID {ex} {ex.GetType()} {ex.Message} {ex.Source}");
			return string.Empty;
		}
	}

	public static string GetUserSteamID()
	{
		try
		{
			return SteamUser.GetSteamID().ToString();
		}
		catch (Exception ex)
		{
			Debug.LogError($"Error on try to get UserSteamID {ex} {ex.GetType()} {ex.Message} {ex.Source}");
			return string.Empty;
		}
	}

	public static bool GetIsDev()
	{
		return false;
	}
}
