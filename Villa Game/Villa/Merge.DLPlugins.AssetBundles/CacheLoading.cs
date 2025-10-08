using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Merge.DLPlugins.AssetBundles;

public class CacheLoading : AssetBundleOperation
{
	private AssetBundleCache asset_bundle_cache;

	public CacheLoading(ref AssetBundleCollection collection, AssetBundleDelegates delegates, AssetBundleCache bundle_cache, MonoBehaviour mono_behaviour)
	{
		asset_bundle_collection = collection;
		asset_bundle_delegates = delegates;
		asset_bundle_cache = bundle_cache;
		owner = mono_behaviour;
	}

	private string PathInCache(string full_bundle_name)
	{
		List<string> list = new List<string>();
		Caching.GetAllCachePaths(list);
		string text = asset_bundle_cache.GetHash(full_bundle_name).ToString();
		string text2 = full_bundle_name.Replace(".sd", "").Replace(".hd", "").Split('/')
			.LastOrDefault();
		return AssetBundleOperation.Combine(list.FirstOrDefault(), text2, text, "__data");
	}

	public bool Exists(BundleName bundle_name)
	{
		return CacheAssetExist(bundle_name);
	}

	public override AssetBundle LoadBundle(BundleName bundle_name, BundleCompletionCallback completion_callback = null)
	{
		string path = PathInCache(bundle_name);
		if (!File.Exists(path))
		{
			return null;
		}
		AssetBundle assetBundle = AssetBundle.LoadFromFile(path);
		asset_bundle_collection.AddRefBundle(bundle_name, assetBundle);
		completion_callback?.Invoke(assetBundle, assetBundle != null);
		return assetBundle;
	}

	public override Coroutine AsyncBundleOperation(BundleName bundle_name, BundleCompletionCallback completion_callback = null)
	{
		return owner.StartCoroutine(AsyncBundleOperationRoutine(bundle_name, completion_callback));
	}

	protected IEnumerator AsyncBundleOperationRoutine(string bundle_name, BundleCompletionCallback completion_callback = null)
	{
		if (asset_bundle_collection.TryGetBundle(bundle_name, out var bundle) && (bool)bundle)
		{
			completion_callback?.Invoke(bundle, result: true);
			yield break;
		}
		asset_bundle_delegates.messegeLog?.Invoke("Start to load bundle from localcache [" + bundle_name + "]");
		bool flag = File.Exists(PathInCache(bundle_name));
		if (!string.IsNullOrEmpty(PathInCache(bundle_name)) && !(PathInCache(bundle_name) == "__data") && flag)
		{
			AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(PathInCache(bundle_name));
			if (request != null)
			{
				yield return request;
			}
			bundle = request?.assetBundle;
			asset_bundle_collection.AddRefBundle(bundle_name, bundle);
			completion_callback?.Invoke(bundle, bundle != null);
		}
	}
}
