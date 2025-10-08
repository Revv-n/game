using System.Collections.Generic;
using UnityEngine;

namespace Merge;

public static class Logs
{
	private struct LogInfo
	{
		public string key;

		public StringColors color;

		public string GetMessage(string message)
		{
			return key.SetFontColor(color) + " >>> " + message;
		}

		public LogInfo(string key, StringColors color)
		{
			this.key = key;
			this.color = color;
		}
	}

	private const string _Core = "Core";

	private const string _Meta = "Meta";

	private const string _Start = "Start";

	private const string _Purchaser = "Purchaser";

	private const string _AssetBundle = "AssetBundle";

	private const string _Localization = "Localization";

	private const string _Analytic = "Analytic";

	private const string _Server = "Server";

	private static readonly Dictionary<string, LogInfo> dict = new Dictionary<string, LogInfo>
	{
		{
			"Core",
			new LogInfo("Core", StringColors.brown)
		},
		{
			"Meta",
			new LogInfo("Meta", StringColors.darkblue)
		},
		{
			"Start",
			new LogInfo("Start", StringColors.orange)
		},
		{
			"Purchaser",
			new LogInfo("Purchaser", StringColors.magenta)
		},
		{
			"AssetBundle",
			new LogInfo("AssetBundle", StringColors.yellow)
		},
		{
			"Analytic",
			new LogInfo("Analytic", StringColors.silver)
		},
		{
			"Localization",
			new LogInfo("Localization", StringColors.cyan)
		},
		{
			"Server",
			new LogInfo("Server", StringColors.green)
		}
	};

	public static void Core(string message)
	{
		TryLog("Core", message);
	}

	public static void Meta(string message)
	{
		TryLog("Meta", message);
	}

	public static void Start(string message)
	{
		TryLog("Start", message);
	}

	public static void Purchaser(string message)
	{
		TryLog("Purchaser", message);
	}

	public static void AssetBundle(string message)
	{
		TryLog("AssetBundle", message);
	}

	public static void Analytic(string message)
	{
		TryLog("Analytic", message);
	}

	public static void Localization(string message)
	{
		TryLog("Localization", message);
	}

	public static void Server(string message)
	{
		TryLog("Server", message);
	}

	public static void SetLogsAllown(string key, bool allown)
	{
		PlayerPrefs.SetInt("AllowLogs_" + key, allown ? 1 : 0);
	}

	public static void SetAllLogsAllown(bool allown)
	{
		foreach (KeyValuePair<string, LogInfo> item in dict)
		{
			SetLogsAllown(item.Key, allown);
		}
	}

	private static void TryLog(string key, string message)
	{
		if (CheckLogsAllown(key))
		{
			Debug.Log(dict[key].GetMessage(message));
		}
	}

	private static bool CheckLogsAllown(string key)
	{
		return PlayerPrefs.GetInt("AllowLogs_" + key, 0) == 1;
	}
}
