using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.Coresdk.Unity;

public class Coresdk
{
	private static Coresdk Instance;

	public string language;

	public string platform;

	private MainThreadDispatcher m_mainThreadDispatcher;

	private Config m_config;

	private DeepLink m_deepLink;

	private AuthClient m_authClient;

	private PaymentClient m_paymentClient;

	public static string Token
	{
		get
		{
			if (Instance == null || Instance.m_authClient == null)
			{
				Debug.LogError("Please first call Coresdk.Initialize().");
				return string.Empty;
			}
			return Instance.m_authClient.Token;
		}
	}

	private static Config Config => Instance.m_config;

	private static AuthClient AuthClient => Instance.m_authClient;

	private static PaymentClient PaymentClient => Instance.m_paymentClient;

	public static IEnumerator Initialize(Language lang, Platform platform, Action<bool> callback = null)
	{
		if (Instance == null)
		{
			GameObject gameObject = new GameObject(typeof(Coresdk).Name);
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			Instance = new Coresdk();
			Instance.m_mainThreadDispatcher = gameObject.AddComponent<MainThreadDispatcher>();
		}
		SetPlatform(platform);
		SetLanguage(lang);
		yield return Instance.CoInit();
		callback?.Invoke(Instance.m_config != null);
	}

	private IEnumerator CoInit()
	{
		ConfigLoader configLoader = new ConfigLoader();
		yield return configLoader.CoInit();
		m_config = configLoader.config;
		m_deepLink = DeepLink.Initialize(m_mainThreadDispatcher);
		m_authClient = new AuthClient(this, m_config, m_deepLink, m_mainThreadDispatcher);
		m_paymentClient = new PaymentClient(this, m_authClient, m_config, m_mainThreadDispatcher);
	}

	public static string GetLoginURL()
	{
		return Config.Domain + "/login/";
	}

	public static void OpenLogin(string game_id, Action<ProfileResult> callback)
	{
		AuthClient.Login(game_id, callback);
	}

	public static void OpenLogout(Action<LogoutResult> callback)
	{
		AuthClient.Logout(callback);
	}

	public static void OpenAccountBindGame(string game_id, string game_account, string jwt, Action<BindProfileResult> callback)
	{
		AuthClient.Bind(game_id, game_account, jwt, callback);
	}

	public static void OpenPayment()
	{
		PaymentClient.Purchase();
	}

	public static void PostAccountBindGame(string game_id, string game_account, Action<AccountBindGameResult> callback)
	{
		Dictionary<string, string> query = new Dictionary<string, string>
		{
			{ "game_id", game_id },
			{ "game_account", game_account },
			{ "platform", Instance.platform }
		};
		PostRequest("/accountBindGame", query, delegate(RawResponse rawResponse)
		{
			AccountBindGameResult obj = AccountBindGameResult.Parse(rawResponse);
			callback(obj);
		});
	}

	public static void SetLanguage(Language lang)
	{
		Instance.language = lang.ToString();
	}

	public static void SetPlatform(Platform platform)
	{
		Instance.platform = platform.ToString();
	}

	public void PostGuestLogin(Action<LoginResult> callback)
	{
		Dictionary<string, string> query = new Dictionary<string, string>();
		PostRequest("/guestLogin", query, delegate(RawResponse rawResponse)
		{
			callback(LoginResult.Parse(rawResponse));
		});
	}

	public void PostLogin(string account, string password, string game_id, Action<LoginResult> callback)
	{
		Dictionary<string, string> query = new Dictionary<string, string>
		{
			{ "account", account },
			{
				"password",
				Base64Util.ToBase64(password)
			},
			{ "game_id", game_id },
			{ "platform", Instance.platform }
		};
		PostRequest("/accountLogin", query, delegate(RawResponse rawResponse)
		{
			callback(LoginResult.Parse(rawResponse));
		});
	}

	public void PostAccountLoginBindGame(string account, string password, string game_id, string game_account, Action<AccountLoginBindGameResult> callback)
	{
		Dictionary<string, string> query = new Dictionary<string, string>
		{
			{ "account", account },
			{
				"password",
				Base64Util.ToBase64(password)
			},
			{ "game_id", game_id },
			{ "game_account", game_account },
			{ "platform", Instance.platform }
		};
		PostRequest("/accountLoginBindGame", query, delegate(RawResponse rawResponse)
		{
			AccountLoginBindGameResult obj = AccountLoginBindGameResult.Parse(rawResponse);
			callback(obj);
		});
	}

	public void PostCheckBindGameStatus(string game_id, Action<CheckBindGameStatusResult> callback)
	{
		Dictionary<string, string> query = new Dictionary<string, string>
		{
			{ "game_id", game_id },
			{ "platform", Instance.platform }
		};
		PostRequest("/checkBindGameStatus", query, delegate(RawResponse rawResponse)
		{
			CheckBindGameStatusResult obj = CheckBindGameStatusResult.Parse(rawResponse);
			callback(obj);
		});
	}

	public void GetProfile(Action<RawResponse> callback)
	{
		Dictionary<string, string> query = new Dictionary<string, string>
		{
			{ "merchantId", Config.MerchantId },
			{ "serviceId", Config.ServiceId },
			{ "platform", Instance.platform }
		};
		GetRequest("/getUserInfo", query, callback);
	}

	private static void GetRequest(string endpoint, Dictionary<string, string> query, Action<RawResponse> callback)
	{
		APIUtil.Get(Config.ApiDomain, endpoint, query, AuthClient.Token, callback, Instance.m_mainThreadDispatcher);
	}

	private static void PostRequest(string endpoint, Dictionary<string, string> query, Action<RawResponse> callback)
	{
		APIUtil.Post(Config.ApiDomain, endpoint, query, AuthClient.Token, callback, Instance.m_mainThreadDispatcher);
	}
}
