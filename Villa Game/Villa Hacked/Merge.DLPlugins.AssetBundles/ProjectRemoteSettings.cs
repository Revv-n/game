using System;
using System.Collections;
using SM.Web.Management;
using UnityEngine;

namespace Merge.DLPlugins.AssetBundles;

public class ProjectRemoteSettings
{
	[Serializable]
	public class DLJson
	{
		public string status;

		public string error;

		public Json data;
	}

	[Serializable]
	public class DLManifestJson
	{
		public string status;

		public string error;

		public DLAssetBundleManifest data;
	}

	[Serializable]
	public class Json
	{
		public string bundle_version;

		public bool force_update;

		public string password;
	}

	public string appBundleName = "com.ceo.merge";

	public string serverUrl { get; } = "https://bundle.match3tv.com";


	public string manifestApi { get; } = "api/manifest";


	public string bundleApi { get; } = "api/bundles";


	public string cohortParam { get; } = "cohort";


	public string testParam { get; } = "test";


	public string settingsFolderName { get; } = "SettingsFiles";


	public string bundlesFolderName { get; } = "Bundles";


	public string settingsFileName { get; } = "settings-" + Application.version + ".json";


	public string platformDirectory => string.Join("/", serverUrl, PlatformName);

	public string testFolderName { get; } = "Testing";


	public string cachedBundlesDirectoryKey { get; } = "cachedBundlesDirectory";


	public string PlatformName => Utility.GetPlatformName();

	public bool GetCachedBundlesDirectory(out string directory)
	{
		if (PlayerPrefs.HasKey(cachedBundlesDirectoryKey))
		{
			directory = PlayerPrefs.GetString(cachedBundlesDirectoryKey);
			return true;
		}
		directory = null;
		return false;
	}

	public void SetCachedBundlesDirectoryFrom(Json json)
	{
		if (json != null)
		{
			PlayerPrefs.SetString(cachedBundlesDirectoryKey, json.bundle_version);
			PlayerPrefs.Save();
		}
	}

	public bool GetBundleVersion(out string directory)
	{
		if (GetCachedBundlesDirectory(out directory))
		{
			return true;
		}
		Debug.LogError("Can't get cached bundles directory!");
		return false;
	}

	public bool GetBundlesDirectory(out string directory)
	{
		directory = serverUrl;
		return true;
	}

	public IEnumerator RemouteSettingsRequest(WebRequestProgressCallback onProgress, WebRequestResultCallback<DLJson> onComplete)
	{
		string text = string.Join("/", serverUrl, manifestApi, appBundleName, PlatformName.ToLower(), Application.version);
		Debug.Log($"PROJECT SETTINGS URL: [{text}]");
		return WebManager.RequestRoutine(text, onProgress, onComplete);
	}
}
