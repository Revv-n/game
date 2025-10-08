using System;
using System.Collections.Generic;
using UnityEngine;

namespace Games.Coresdk.Unity;

public class AuthClient
{
	private const string PREF_KEY_TOKEN = "Coresdk.Unity.Token";

	private Config m_config;

	private string m_prefToken;

	private DeepLink m_deepLink;

	private MainThreadDispatcher m_mainThreadDispatcher;

	private Coresdk m_sdk;

	private Action<ProfileResult> m_loginCallback;

	private Action<LogoutResult> m_logoutResult;

	private Action<BindProfileResult> m_bindGameResult;

	public string Token
	{
		get
		{
			return m_prefToken;
		}
		private set
		{
			if (value == null)
			{
				m_prefToken = value;
				PlayerPrefs.DeleteKey("Coresdk.Unity.Token");
			}
			else if (m_prefToken != value)
			{
				m_prefToken = value;
				PlayerPrefs.SetString("Coresdk.Unity.Token", value);
			}
		}
	}

	public AuthClient(Coresdk sdk, Config config, DeepLink deepLink, MainThreadDispatcher mainThreadDispatcher)
	{
		m_sdk = sdk;
		m_config = config;
		m_deepLink = deepLink;
		m_prefToken = PlayerPrefs.GetString("Coresdk.Unity.Token", "");
		m_mainThreadDispatcher = mainThreadDispatcher;
	}

	public void Login(string game_id, Action<ProfileResult> callback)
	{
		m_loginCallback = callback;
		if (string.IsNullOrEmpty(Token))
		{
			StartLoginDeepLink(game_id);
			return;
		}
		GetProfile(delegate(ProfileResult profileResult)
		{
			if (profileResult.IsSuccess)
			{
				m_loginCallback(profileResult);
			}
			else
			{
				PlayerPrefs.DeleteKey("Coresdk.Unity.Token");
				StartLoginDeepLink(game_id);
			}
		});
	}

	private void StartLoginDeepLink(string game_id)
	{
		string deepLinkURL = GetDeepLinkURL("login", new Dictionary<string, string>
		{
			{ "login", "true" },
			{ "game_id", game_id },
			{ "lang", m_sdk.language },
			{ "platform", m_sdk.platform }
		});
		m_deepLink.OpenURL(deepLinkURL, OnLoginDeepLinkCallback);
	}

	private string GetDeepLinkURL(string endpoint, Dictionary<string, string> queries)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>
		{
			{ "merchantId", m_config.MerchantId },
			{ "serviceId", m_config.ServiceId },
			{
				"callback",
				URLUtility.GetDeepLinkURL(Application.identifier)
			}
		};
		foreach (KeyValuePair<string, string> query in queries)
		{
			dictionary[query.Key] = query.Value;
		}
		string queryString = APIUtil.GetQueryString(dictionary);
		return $"{m_config.Domain}/{endpoint}/?{queryString}";
	}

	private void OnLoginDeepLinkCallback(string url)
	{
		string authorizationToken = URLUtility.GetAuthorizationToken(url);
		Token = authorizationToken;
		GetProfile(m_loginCallback);
	}

	public void Logout(Action<LogoutResult> callback)
	{
		m_logoutResult = callback;
		StartLogoutDeepLink();
	}

	private void StartLogoutDeepLink()
	{
		string deepLinkURL = GetDeepLinkURL("login", new Dictionary<string, string>
		{
			{ "logout", "true" },
			{ "lang", m_sdk.language },
			{ "platform", m_sdk.platform }
		});
		m_deepLink.OpenURL(deepLinkURL, delegate
		{
			Token = "";
			m_logoutResult(new LogoutResult());
		});
	}

	public void Bind(string game_id, string game_account, string jwt, Action<BindProfileResult> callback)
	{
		m_bindGameResult = callback;
		StartAccountBindGameDeepLink(game_id, game_account, jwt);
	}

	private void StartAccountBindGameDeepLink(string game_id, string game_account, string jwt)
	{
		string deepLinkURL = GetDeepLinkURL("bind", new Dictionary<string, string>
		{
			{ "game_id", game_id },
			{ "game_account", game_account },
			{ "lang", m_sdk.language },
			{ "platform", m_sdk.platform },
			{ "jwt", jwt }
		});
		m_deepLink.OpenURL(deepLinkURL, OnBindDeepLinkCallback);
	}

	private void OnBindDeepLinkCallback(string url)
	{
		TokenCollection tokenCollection = TokenCollection.Parse(url);
		string value = tokenCollection.GetValue("token");
		bool bindResult = tokenCollection.GetValue("bind") == "success";
		if (bindResult)
		{
			Token = value;
		}
		GetProfile(delegate(ProfileResult _)
		{
			BindProfileResult obj = BindProfileResult.Parse(_, bindResult);
			m_bindGameResult(obj);
		});
	}

	public void GetProfile(Action<ProfileResult> callback)
	{
		m_sdk.GetProfile(delegate(RawResponse rawResponse)
		{
			callback(ProfileResult.Parse(rawResponse));
		});
	}
}
