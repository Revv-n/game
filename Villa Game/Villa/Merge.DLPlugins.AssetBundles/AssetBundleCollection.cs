using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Merge.DLPlugins.AssetBundles;

[Serializable]
public class AssetBundleCollection
{
	private DLAssetBundleManifest m_Manifest;

	public DLAssetBundleManifest Manifest
	{
		get
		{
			if (m_Manifest == null)
			{
				m_Manifest = DLAssetBundleManifest.Read();
			}
			return m_Manifest;
		}
		private set
		{
			m_Manifest = value;
		}
	}

	public Dictionary<string, AssetBundle> BundlesRefCollection { get; private set; } = new Dictionary<string, AssetBundle>();


	internal Dictionary<string, BundleLoading> BundleLoadingsDictionary { get; private set; } = new Dictionary<string, BundleLoading>();


	public Queue<BundleLoadingInfo> LoadingQueue { get; private set; } = new Queue<BundleLoadingInfo>();


	public bool TryGetBundle(BundleName bundleName, out AssetBundle bundle)
	{
		return (bundle = GetBundle(bundleName)) != null;
	}

	public AssetBundle GetBundle(BundleName bundleName)
	{
		AssetBundle value = null;
		BundlesRefCollection.TryGetValue(bundleName, out value);
		if (value == null)
		{
			return null;
		}
		return value;
	}

	public void SetManifest(DLAssetBundleManifest value)
	{
		Manifest = value;
	}

	public void EnqueueAsyncBundleLoading(params BundleLoadingInfo[] bundleLoaders)
	{
		BundleLoadingInfo[] source = LoadingQueue.OrderBy((BundleLoadingInfo x) => x.NormalizedSize).ToArray();
		foreach (BundleLoadingInfo bundle in bundleLoaders)
		{
			if (source.All((BundleLoadingInfo obj) => obj.Name != bundle.Name))
			{
				LoadingQueue.Enqueue(bundle);
			}
		}
	}

	public void AddRefBundle(string bundleName, AssetBundle bundle)
	{
		if (!(bundle == null))
		{
			if (BundlesRefCollection.ContainsKey(bundleName))
			{
				UnloadBundle(new BundleName(bundleName), unload_all_loaded_objects: true);
				BundlesRefCollection[bundleName] = bundle;
			}
			else
			{
				BundlesRefCollection.Add(bundleName, bundle);
			}
		}
	}

	public bool RemoveRefBundle(string bundleName)
	{
		if (!BundlesRefCollection.ContainsKey(bundleName))
		{
			return false;
		}
		UnloadBundle(new BundleName(bundleName), unload_all_loaded_objects: true);
		BundlesRefCollection.Remove(bundleName);
		return true;
	}

	internal void AddLoadingBundle(string bundleName, BundleLoading bundle)
	{
		if (bundle != null)
		{
			if (BundleLoadingsDictionary.ContainsKey(bundleName))
			{
				UnloadBundle(new BundleName(bundleName), unload_all_loaded_objects: true);
				BundleLoadingsDictionary[bundleName] = bundle;
			}
			else
			{
				BundleLoadingsDictionary.Add(bundleName, bundle);
			}
		}
	}

	public bool RemoveLoadingundle(string bundleName)
	{
		if (!BundleLoadingsDictionary.ContainsKey(bundleName))
		{
			return false;
		}
		BundleLoadingsDictionary.Remove(bundleName);
		return true;
	}

	public bool Contains(string bundleName)
	{
		return BundlesRefCollection.ContainsKey(bundleName);
	}

	public bool BundleIsLoaded(BundleName bundleName)
	{
		return Contains(bundleName);
	}

	public void ClearAll()
	{
		UnloadAll();
		Caching.ClearCache();
	}

	public void UnloadAll()
	{
		foreach (AssetBundle value in BundlesRefCollection.Values)
		{
			if (!(value == null))
			{
				value.Unload(unloadAllLoadedObjects: true);
			}
		}
		BundlesRefCollection.Clear();
		LoadingQueue.Clear();
	}

	public void UnloadBundle(BundleName bundle_name, bool unload_all_loaded_objects = false)
	{
		if (TryGetBundle(bundle_name, out var bundle))
		{
			bundle.Unload(unload_all_loaded_objects);
			bundle = null;
		}
	}

	public void SavePlanBundleCollection(string key)
	{
	}

	public void PrintCollection()
	{
		if (!Application.isPlaying)
		{
			Debug.LogError("Необходимо выполнять в рантайме!");
			return;
		}
		foreach (DLAssetBundleObject allObject in Manifest.AllObjects)
		{
			bool flag = Contains(allObject.Name);
			if (!flag)
			{
				_ = Color.green;
			}
			else
			{
				_ = Color.red;
			}
			BaseLogger<AssetBundleManager.Logger>.Log(string.Concat("Бандл [" + (flag ? "отсутствует в кэше" : "кэширован") + "]", $" [{allObject.Url}/{allObject.Name} | {Manifest.GetAssetBundleHash(allObject.Name)}]"));
		}
	}
}
