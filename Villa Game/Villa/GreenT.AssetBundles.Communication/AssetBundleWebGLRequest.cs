using System.Collections;
using UnityEngine;

namespace GreenT.AssetBundles.Communication;

public class AssetBundleWebGLRequest : AssetBundleRequest
{
	public AssetBundleWebGLRequest(AssetBundleRequestData data, AssetBundle previewBundle = null)
		: base(data, previewBundle)
	{
	}

	public override IEnumerator Send()
	{
		yield return base.Send();
		yield return DownloadManifest();
		if (!IsExitedError)
		{
			yield return DownloadBundle();
			if (!IsExitedError)
			{
				AssetBundleRequest.assetBundles[data.name] = base.Response.bundle;
			}
		}
	}

	protected override bool TrySetBundleFromCache(out CachedAssetBundle cachedAssetBundle)
	{
		cachedAssetBundle = new CachedAssetBundle(data.name, base.Response.info.bundleHash);
		Caching.ClearOtherCachedVersions(data.name, base.Response.info.bundleHash);
		if (!AssetBundleRequest.assetBundles.TryGetValue(data.name, out var value))
		{
			return false;
		}
		if (value.name.Contains(".sd"))
		{
			return false;
		}
		base.Response.bundle = value;
		return true;
	}
}
