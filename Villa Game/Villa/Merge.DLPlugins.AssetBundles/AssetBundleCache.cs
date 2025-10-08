using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Merge.DLPlugins.AssetBundles;

[Serializable]
public class AssetBundleCache
{
	protected AssetBundleCollection asset_bundle_collection;

	public AssetBundleCache(AssetBundleCollection collection)
	{
		asset_bundle_collection = collection;
	}

	public bool ActualBundleIsInCache(BundleName bundle_name)
	{
		if (!asset_bundle_collection.Manifest)
		{
			return false;
		}
		List<Hash128> list = new List<Hash128>();
		Caching.GetCachedVersions(bundle_name, list);
		return asset_bundle_collection.Manifest.GetAssetBundleHash(bundle_name) == list.LastOrDefault();
	}

	public bool BundleIsInCache(BundleName bundle_name)
	{
		List<Hash128> list = new List<Hash128>();
		Caching.GetCachedVersions(bundle_name, list);
		return list.Any();
	}

	public Hash128 GetHash(string bundleName)
	{
		if ((bool)asset_bundle_collection.Manifest)
		{
			return asset_bundle_collection.Manifest.GetAssetBundleHash(bundleName);
		}
		List<Hash128> list = new List<Hash128>();
		Caching.GetCachedVersions(bundleName, list);
		return list.FirstOrDefault();
	}
}
