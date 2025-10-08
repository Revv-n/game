using System.Collections;
using System.IO;
using UnityEngine;

namespace Merge.DLPlugins.AssetBundles;

public class StreamingAssetsLoading : AssetBundleOperation
{
	public StreamingAssetsLoading(ref AssetBundleCollection collection, AssetBundleDelegates delegates, MonoBehaviour mono_behaviour)
	{
		asset_bundle_collection = collection;
		asset_bundle_delegates = delegates;
		owner = mono_behaviour;
	}

	private string PathInStreamingAssets(string full_bundle_name)
	{
		return AssetBundleOperation.Combine(AssetBundleOperation.PathStreamingAsset, full_bundle_name);
	}

	public bool Exists(BundleName bundle_name)
	{
		return StreamingAssetExists(bundle_name);
	}

	public override AssetBundle LoadBundle(BundleName bundle_name, BundleCompletionCallback completion_callback = null)
	{
		string text = PathInStreamingAssets(bundle_name);
		if (!File.Exists(text))
		{
			Debug.Log("No file " + text);
			return null;
		}
		AssetBundle assetBundle = AssetBundle.LoadFromFile(text);
		if (assetBundle == null)
		{
			Debug.Log("Loading error in SA " + text);
		}
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
		asset_bundle_delegates.messegeLog?.Invoke("Start to load bundle from StreamingAssets: [" + bundle_name + "]");
		bool flag = File.Exists(PathInStreamingAssets(bundle_name));
		if (!string.IsNullOrEmpty(PathInStreamingAssets(bundle_name)) && flag)
		{
			AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(PathInStreamingAssets(bundle_name));
			if (request != null)
			{
				yield return request;
			}
			AssetBundle assetBundle = request?.assetBundle;
			asset_bundle_collection.AddRefBundle(bundle_name, assetBundle);
			completion_callback?.Invoke(assetBundle, assetBundle != null);
		}
	}
}
