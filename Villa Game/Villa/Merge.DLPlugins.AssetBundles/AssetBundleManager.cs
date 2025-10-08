using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Merge.DLPlugins.AssetBundles;

[RequireComponent(typeof(UniqueComponent))]
public class AssetBundleManager : MonoBehaviour
{
	public class Logger : BaseLogger<Logger>
	{
		public Logger()
		{
			Key = "AssetBundle";
			SendLogs = AssetBundleManager.Instance.m_SendBuildLogs;
			colorKey = Color.green;
			colorText = Color.yellow;
		}
	}

	private static AssetBundleManager instance;

	[SerializeField]
	private bool m_SendBuildLogs;

	private AssetBundleCollection asset_bundle_collection;

	private AssetBundleDelegates asset_bundle_delegates;

	private AssetBundleCache asset_bundle_cache;

	private ProjectRemoteSettings project_remote_settings;

	private ManifestLoading manifest_loading;

	private RequestLoading request_loading;

	private StreamingAssetsLoading streaming_assets_loading;

	private CacheLoading cache_loading;

	private LocalLoading local_loading;

	private bool IsInitialized;

	public static AssetBundleManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = UnityEngine.Object.FindObjectOfType<AssetBundleManager>();
				if (instance == null)
				{
					instance = new GameObject("AssetBundleManager").AddComponent<AssetBundleManager>();
				}
				instance.Initialization();
			}
			return instance;
		}
	}

	public bool AllBundleInited { get; set; }

	public AssetBundleDelegates bundle_delegates => asset_bundle_delegates;

	public void Initialization()
	{
		if (!IsInitialized)
		{
			IsInitialized = true;
			project_remote_settings = new ProjectRemoteSettings();
			asset_bundle_collection = new AssetBundleCollection();
			asset_bundle_delegates = new AssetBundleDelegates();
			asset_bundle_cache = new AssetBundleCache(asset_bundle_collection);
			AssetBundleDelegates assetBundleDelegates = asset_bundle_delegates;
			assetBundleDelegates.messegeLog = (MessegeLog)Delegate.Combine(assetBundleDelegates.messegeLog, new MessegeLog(BaseLogger<Logger>.Log));
			AssetBundleDelegates assetBundleDelegates2 = asset_bundle_delegates;
			assetBundleDelegates2.messegeLogError = (MessegeLogError)Delegate.Combine(assetBundleDelegates2.messegeLogError, new MessegeLogError(BaseLogger<Logger>.LogError));
			manifest_loading = new ManifestLoading(ref asset_bundle_collection, asset_bundle_delegates, this, project_remote_settings);
			request_loading = new RequestLoading(ref asset_bundle_collection, asset_bundle_delegates, this);
			streaming_assets_loading = new StreamingAssetsLoading(ref asset_bundle_collection, asset_bundle_delegates, this);
			cache_loading = new CacheLoading(ref asset_bundle_collection, asset_bundle_delegates, asset_bundle_cache, this);
			local_loading = new LocalLoading(ref asset_bundle_collection, asset_bundle_delegates, this, cache_loading, streaming_assets_loading);
		}
	}

	public void Clear(string bundleName, bool unload_all_loaded_objects = true)
	{
		if (!(asset_bundle_collection.Manifest == null))
		{
			UnloadBundle(bundleName, unload_all_loaded_objects);
			if (IsExistsInCache(bundleName) && Caching.ClearCachedVersion(bundleName, asset_bundle_collection.Manifest.GetAssetBundleHash(bundleName)))
			{
				asset_bundle_collection.RemoveRefBundle(bundleName);
				BaseLogger<Logger>.Log("Bundle удален [" + bundleName + "]");
			}
		}
	}

	public bool TryClearCachedStreamingAssetsBundles()
	{
		AssetBundlesConfig assetBundlesConfig = Resources.Load<AssetBundlesConfig>("AssetBundlesConfig");
		if (assetBundlesConfig == null)
		{
			return false;
		}
		List<string> streamingAssetBundles = assetBundlesConfig.StreamingAssetBundles;
		if (streamingAssetBundles.IsNullOrEmpty())
		{
			return false;
		}
		int num = 0;
		foreach (string item in streamingAssetBundles)
		{
			if (IsExistsInCache(item) && IsExistsInStreamingAssets(item))
			{
				num++;
				Clear(item);
			}
		}
		if (num > 0)
		{
			BaseLogger<Logger>.Log($"Удалено {num} бандлов из кэша, которые уже есть в StreamingAssets");
		}
		return num > 0;
	}

	public bool CacheIsEmpty()
	{
		List<string> streamingAssetBundles = Resources.Load<AssetBundlesConfig>("AssetBundlesConfig").StreamingAssetBundles;
		if (streamingAssetBundles.IsNullOrEmpty())
		{
			return true;
		}
		return !streamingAssetBundles.Any((string x) => IsExistsInCache(x));
	}

	public bool IsExistsInStreamingAssets(BundleName bundle_name)
	{
		return streaming_assets_loading.Exists(bundle_name);
	}

	public bool IsExistsInCache(BundleName bundle_name)
	{
		return cache_loading.Exists(bundle_name);
	}

	public Coroutine DownloadManifest(BundleProgressCallback progress_callback = null, ManifestCompletionCallback completion_callback = null)
	{
		return manifest_loading.DownloadPlaformManifest(progress_callback, completion_callback);
	}

	public Coroutine ReadManifest(BundleProgressCallback progress_callback = null, ManifestCompletionCallback completion_callback = null)
	{
		return manifest_loading.ReadPlaformManifest(progress_callback, completion_callback);
	}

	public Coroutine RequestAllBundlesDownload(Action<bool> is_already_cached = null, BundleProgressCallback progress_callback = null, BundleCompletionCallback completion_callback = null)
	{
		return request_loading.AsyncLoadingAllBundles(is_already_cached, progress_callback, completion_callback);
	}

	public Coroutine StreamingBundlesLoading(BundleName bundleName, BundleCompletionCallback completion_callback = null)
	{
		return streaming_assets_loading.AsyncBundleOperation(bundleName, completion_callback);
	}

	public Coroutine LocalBundleLoading(BundleName bundleName, BundleCompletionCallback completion_callback = null)
	{
		return local_loading.AsyncBundleOperation(bundleName, completion_callback);
	}

	public Coroutine RequestBundleDownload(BundleName bundle_name, BundleCompletionCallback completion_callback = null)
	{
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			return LocalBundleLoading(bundle_name, completion_callback);
		}
		return request_loading.AsyncBundleOperation(bundle_name, completion_callback);
	}

	public Coroutine LocalBundleCollectionLoading(string[] bundle_names, BundleCompletionCallback completion_callback)
	{
		return local_loading.AssyncLoadingBundleCollectionOperation(bundle_names, completion_callback);
	}

	public bool TryGetBundle(BundleName bundleName, out AssetBundle bundle, bool skipStreamingAssets = false)
	{
		local_loading.skipStreamingAssets = skipStreamingAssets;
		if (asset_bundle_collection.TryGetBundle(bundleName, out bundle))
		{
			return true;
		}
		Debug.Log($"AssetBundles >>> Not found {bundleName} in AB. Try load in SA [skipStreamingAssets: {skipStreamingAssets}]");
		return local_loading.TryLoadBundle(bundleName, out bundle);
	}

	public Coroutine GetBundleAsync(BundleName bundle_name, BundleCompletionCallback completion_callback = null)
	{
		if (asset_bundle_collection.TryGetBundle(bundle_name, out var bundle))
		{
			completion_callback?.Invoke(bundle, result: true);
			return null;
		}
		return local_loading.AsyncBundleOperation(bundle_name, completion_callback);
	}

	public void ClearAll()
	{
		asset_bundle_collection.ClearAll();
	}

	public void UnloadAll()
	{
		asset_bundle_collection.UnloadAll();
	}

	public void UnloadBundle(BundleName bundle_name, bool unload_all_loaded_objects = false)
	{
		asset_bundle_collection.UnloadBundle(bundle_name, unload_all_loaded_objects);
	}

	public void SaveBundleQueue(string key)
	{
		asset_bundle_collection.SavePlanBundleCollection(key);
	}

	public void PrintCollection()
	{
		asset_bundle_collection.PrintCollection();
	}
}
