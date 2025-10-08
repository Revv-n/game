using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using SM.Web.Management;
using UnityEngine;

namespace Merge.DLPlugins.AssetBundles;

[Serializable]
public class ManifestLoading
{
	protected ProjectRemoteSettings project_remote_settings;

	protected AssetBundleCollection asset_bundle_collection;

	protected AssetBundleDelegates asset_bundle_delegates;

	protected MonoBehaviour owner;

	private const string manifestName = "dlassetbundlemanifest.json";

	public ManifestLoading(ref AssetBundleCollection collection, AssetBundleDelegates delegates, MonoBehaviour mono_behaviour, ProjectRemoteSettings remoteSettings)
	{
		asset_bundle_collection = collection;
		asset_bundle_delegates = delegates;
		owner = mono_behaviour;
		project_remote_settings = remoteSettings;
	}

	public Coroutine DownloadPlaformManifest(BundleProgressCallback progress_callback = null, ManifestCompletionCallback completion_callback = null)
	{
		return owner.StartCoroutine(DownloadManifest(progress_callback, completion_callback));
	}

	private IEnumerator DownloadManifest(BundleProgressCallback progress_callback = null, ManifestCompletionCallback completion_callback = null)
	{
		asset_bundle_delegates.messegeLog("Start loading manifests");
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();
		yield return BundleManifestRequest("", testingValue: false, delegate
		{
		}, delegate(ProjectRemoteSettings.DLManifestJson content, bool result, string error)
		{
			DLAssetBundleManifest dLAssetBundleManifest = null;
			if (result)
			{
				dLAssetBundleManifest = content.data;
				InitManifest(dLAssetBundleManifest);
				stopwatch.Stop();
				if (dLAssetBundleManifest == new DLAssetBundleManifest())
				{
					asset_bundle_delegates.messegeLogError($"Empty manifest {stopwatch.Elapsed}");
				}
				else if (dLAssetBundleManifest == null)
				{
					asset_bundle_delegates.messegeLogError($"No Manifest {stopwatch.Elapsed}");
				}
				else
				{
					asset_bundle_delegates.messegeLog($"Manifests downloaded {stopwatch.Elapsed}");
				}
				asset_bundle_collection.SetManifest(dLAssetBundleManifest);
				completion_callback?.Invoke(dLAssetBundleManifest, result);
			}
			completion_callback?.Invoke(dLAssetBundleManifest, result);
		});
	}

	private IEnumerator BundleManifestRequest(string cohortValue, bool testingValue, WebRequestProgressCallback onProgress, WebRequestResultCallback<ProjectRemoteSettings.DLManifestJson> onComplete)
	{
		UriBuilder uriBuilder = new UriBuilder(string.Join("/", project_remote_settings.serverUrl, project_remote_settings.bundleApi, project_remote_settings.appBundleName, project_remote_settings.PlatformName.ToLower(), Application.version));
		uriBuilder.Query += $"{project_remote_settings.cohortParam}={cohortValue}&";
		uriBuilder.Query += string.Format("{0}={1}", project_remote_settings.testParam, testingValue ? "1" : "0");
		return WebManager.RequestRoutine(uriBuilder.Uri.ToString(), onProgress, onComplete);
	}

	public Coroutine ReadPlaformManifest(BundleProgressCallback progress_callback = null, ManifestCompletionCallback completion_callback = null)
	{
		return owner.StartCoroutine(ReadManifest(progress_callback, completion_callback));
	}

	private IEnumerator ReadManifest(BundleProgressCallback progress_callback = null, ManifestCompletionCallback completion_callback = null)
	{
		DLAssetBundleManifest manifest = Read("dlassetbundlemanifest.json");
		yield return null;
		bool result = manifest != null;
		asset_bundle_collection.SetManifest(manifest);
		completion_callback?.Invoke(null, result);
	}

	public void InitManifest(DLAssetBundleManifest manifest)
	{
		if (manifest == null)
		{
			return;
		}
		foreach (KeyValuePair<string, List<DLAssetBundleObject>> item in manifest)
		{
			manifest.AllObjects.AddRange(item.Value);
		}
		Write(manifest, "dlassetbundlemanifest.json");
	}

	private DLAssetBundleManifest Read(string name)
	{
		string path = Path.Combine(BinarySerializer.GetFullPath(name));
		if (!File.Exists(path))
		{
			return new DLAssetBundleManifest();
		}
		try
		{
			DLAssetBundleManifest dLAssetBundleManifest = JsonConvert.DeserializeObject<DLAssetBundleManifest>(File.ReadAllText(path));
			InitManifest(dLAssetBundleManifest);
			return dLAssetBundleManifest;
		}
		catch (Exception exception)
		{
			UnityEngine.Debug.LogError("HASH MANIFEST LOG: Trouble with parsing json from file, return clean manifest!");
			UnityEngine.Debug.LogException(exception);
			return new DLAssetBundleManifest();
		}
	}

	private void Write(DLAssetBundleManifest manifest, string name)
	{
		string path = Path.Combine(BinarySerializer.GetFullPath(name));
		string s = JsonConvert.SerializeObject(manifest, Formatting.Indented);
		using FileStream fileStream = File.Open(path, FileMode.Create);
		byte[] bytes = Encoding.UTF8.GetBytes(s);
		fileStream.Write(bytes, 0, bytes.Length);
	}
}
