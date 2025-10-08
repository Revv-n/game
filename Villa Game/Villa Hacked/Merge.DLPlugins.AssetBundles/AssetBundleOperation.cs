using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Merge.DLPlugins.AssetBundles;

public abstract class AssetBundleOperation
{
	protected AssetBundleCollection asset_bundle_collection;

	protected AssetBundleDelegates asset_bundle_delegates;

	protected MonoBehaviour owner;

	public static string PathStreamingAsset => Combine(Application.streamingAssetsPath, "", PlatformsUtility.GetCurrentPlatformFolderName());

	public virtual bool TryLoadBundle(BundleName bundle_name, out AssetBundle bundle, BundleCompletionCallback completion_callback = null)
	{
		return (UnityEngine.Object)(object)(bundle = LoadBundle(bundle_name, completion_callback)) != null;
	}

	public abstract AssetBundle LoadBundle(BundleName bundle_name, BundleCompletionCallback completion_callback = null);

	public abstract Coroutine AsyncBundleOperation(BundleName bundle_name, BundleCompletionCallback completion_callback = null);

	public bool CacheAssetExist(string bundle_name)
	{
		string text = bundle_name;
		if (text.Contains('/'))
		{
			text = bundle_name.Split('/', StringSplitOptions.None).Last();
		}
		List<Hash128> list = new List<Hash128>();
		Caching.GetCachedVersions(text, list);
		return list.Any();
	}

	public bool StreamingAssetExists(string bundle_name)
	{
		return File.Exists(Combine(PathStreamingAsset, bundle_name));
	}

	public static string Combine(params string[] args)
	{
		return Path.Combine(args).Replace('\\', '/');
	}
}
