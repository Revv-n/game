using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public static class SteamDataSuite
{
	private class SteamDataSuiteRunner : MonoBehaviour
	{
	}

	private const string PLAYER_PREF_KEY = "steamDataSuiteSuccess";

	private const string PLAYER_PREF_KEY_HASH = "steamDataSuiteKeyHash";

	private const string ApiUrl = "http://steam.gs/ca/{TrackingKey}";

	private static SteamDataSuiteRunner _instance;

	public static void Initialize()
	{
		InternalInit();
		SteamDataSuiteConfig[] array = Resources.LoadAll<SteamDataSuiteConfig>("");
		if (array.Length == 0)
		{
			Debug.LogError("SteamDataSuite Error: Unable to find any Config-files. Please create a new file in your Resources-folder");
		}
		else if (!array[0].Enabled)
		{
			Debug.LogWarning("SteamDataSuite Disabled!");
		}
		else
		{
			SendForm(array[0]);
		}
	}

	private static void InternalInit()
	{
		if (_instance == null)
		{
			_instance = new GameObject("SteamDataSuite", typeof(SteamDataSuiteRunner)).GetComponent<SteamDataSuiteRunner>();
			_instance.hideFlags = HideFlags.HideAndDontSave;
		}
	}

	private static void SendForm(SteamDataSuiteConfig config)
	{
		if (PlayerPrefs.HasKey("steamDataSuiteSuccess") && PlayerPrefs.GetInt("steamDataSuiteKeyHash", 0) == (config.TrackingKey?.GetHashCode() ?? 0))
		{
			if (config.DebugMode)
			{
				Debug.Log("SteamDataSuite: Event already registered on server!");
			}
		}
		else
		{
			_instance.StartCoroutine(HandleFormPost(config));
		}
	}

	private static IEnumerator HandleFormPost(SteamDataSuiteConfig config)
	{
		List<IMultipartFormSection> list = new List<IMultipartFormSection>();
		string text = "http://steam.gs/ca/{TrackingKey}".Replace("{TrackingKey}", config.TrackingKey);
		UnityWebRequest www = UnityWebRequest.Post(text, list);
		yield return www.SendWebRequest();
		if (www.isNetworkError || www.isHttpError)
		{
			if (config.DebugMode)
			{
				Debug.LogError("SteamDataSuite Error: " + www.error);
			}
		}
		else if (www.isDone && www.responseCode == 200)
		{
			PlayerPrefs.SetInt("steamDataSuiteSuccess", 1);
			PlayerPrefs.SetInt("steamDataSuiteKeyHash", config.TrackingKey?.GetHashCode() ?? 0);
			PlayerPrefs.Save();
			if (config.DebugMode)
			{
				Debug.Log("SteamDataSuite Successful call");
				Debug.Log(www.downloadHandler.text);
			}
		}
		www.Dispose();
	}
}
