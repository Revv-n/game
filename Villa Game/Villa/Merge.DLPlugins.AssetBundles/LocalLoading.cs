using System;
using System.Collections;
using UnityEngine;

namespace Merge.DLPlugins.AssetBundles;

public class LocalLoading : AssetBundleOperation
{
	private CacheLoading cache_loading;

	private StreamingAssetsLoading streaming_assets_loading;

	private float progress_total;

	private float progress_step;

	public bool skipStreamingAssets;

	public LocalLoading(ref AssetBundleCollection collection, AssetBundleDelegates delegates, MonoBehaviour mono_behaviour, CacheLoading _cache_loading, StreamingAssetsLoading stream_loading)
	{
		asset_bundle_collection = collection;
		asset_bundle_delegates = delegates;
		owner = mono_behaviour;
		cache_loading = _cache_loading;
		streaming_assets_loading = stream_loading;
	}

	public override bool TryLoadBundle(BundleName bundle_name, out AssetBundle bundle, BundleCompletionCallback completion_callback = null)
	{
		AssetBundle assetBundle = (bundle = LoadBundle(bundle_name, completion_callback));
		Debug.Log($"Bundle {bundle_name} in SA: {assetBundle != null}");
		return assetBundle != null;
	}

	public override AssetBundle LoadBundle(BundleName bundle_name, BundleCompletionCallback completion_callback = null)
	{
		AssetBundle bundle = null;
		bool flag = false;
		if (CacheAssetExist(bundle_name))
		{
			flag = cache_loading.TryLoadBundle(bundle_name, out bundle, completion_callback);
		}
		else if (skipStreamingAssets)
		{
			Debug.Log($"Skip SA for {bundle_name}");
			skipStreamingAssets = false;
		}
		else
		{
			Debug.Log($"Loading {bundle_name} in SA");
			flag = streaming_assets_loading.TryLoadBundle(bundle_name, out bundle, completion_callback);
		}
		if (!flag)
		{
			asset_bundle_delegates.messegeLogError?.Invoke($"Bundle with name [{bundle_name}] is NULL!");
		}
		return bundle;
	}

	public Coroutine AssyncLoadingBundleCollectionOperation(string[] bundle_names, BundleCompletionCallback completion_callback)
	{
		return owner.StartCoroutine(AssyncLoadingBundleCollection(bundle_names, completion_callback));
	}

	private IEnumerator AssyncLoadingBundleCollection(string[] bundle_names, BundleCompletionCallback completion_callback)
	{
		progress_step = 1f / (float)bundle_names.Length;
		Mathf.CeilToInt((float)bundle_names.Length / 10f);
		foreach (string info in bundle_names)
		{
			if (string.IsNullOrEmpty(info))
			{
				continue;
			}
			if (asset_bundle_collection.Contains(info))
			{
				progress_total = Mathf.Clamp(progress_total + progress_step, 0f, 1f);
				continue;
			}
			owner.StartCoroutine(AsyncBundleOperationRoutine(info, delegate(AssetBundle bundle, bool result)
			{
				progress_total = Mathf.Clamp(progress_total + progress_step, 0f, 1f);
				if (!asset_bundle_collection.Contains(info))
				{
					asset_bundle_collection.BundlesRefCollection.Add(info, bundle);
				}
			}));
		}
		while (progress_total < 0.99f)
		{
			yield return new WaitForEndOfFrame();
		}
		completion_callback?.Invoke(null, result: true);
	}

	public override Coroutine AsyncBundleOperation(BundleName bundle_name, BundleCompletionCallback completion_callback)
	{
		return owner.StartCoroutine(AsyncBundleOperationRoutine(bundle_name, completion_callback));
	}

	protected IEnumerator AsyncBundleOperationRoutine(string bundle_name, BundleCompletionCallback completion_callback)
	{
		if (string.IsNullOrEmpty(bundle_name))
		{
			completion_callback?.Invoke(null, result: false);
			yield break;
		}
		bool res = false;
		AssetBundle bundle = null;
		completion_callback = (BundleCompletionCallback)Delegate.Combine(completion_callback, (BundleCompletionCallback)delegate(AssetBundle b, bool r)
		{
			res = r;
			bundle = b;
		});
		yield return cache_loading.AsyncBundleOperation(bundle_name, completion_callback);
		if (!res)
		{
			yield return streaming_assets_loading.AsyncBundleOperation(bundle_name, completion_callback);
			if (!res)
			{
				asset_bundle_delegates.messegeLogError?.Invoke("Bundle with name [" + bundle_name + "] is NULL!");
			}
		}
	}
}
