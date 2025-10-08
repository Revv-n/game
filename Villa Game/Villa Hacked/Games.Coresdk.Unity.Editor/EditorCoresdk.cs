using System;
using System.Reflection;

namespace Games.Coresdk.Unity.Editor;

public class EditorCoresdk
{
	private static Coresdk _sdk;

	private static Coresdk sdk
	{
		get
		{
			if (_sdk != null)
			{
				return _sdk;
			}
			_sdk = (Coresdk)typeof(Coresdk).GetField("Instance", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
			return _sdk;
		}
	}

	public static void PostLogin(string account, string password, string game_id, Action<LoginResult> callback)
	{
		sdk.PostLogin(account, password, game_id, callback);
	}

	public static void PostAccountBindGame(string account, string password, string game_id, string game_account, Action<AccountLoginBindGameResult> callback)
	{
		sdk.PostAccountLoginBindGame(account, password, game_id, game_account, callback);
	}

	public static void PostGuestLogin(Action<LoginResult> callback)
	{
		sdk.PostGuestLogin(callback);
	}
}
