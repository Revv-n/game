using System;
using System.Collections;
using GreenT.HornyScapes;
using UnityEngine;
using UnityEngine.Networking;

namespace GreenT.AssetBundles.Communication;

public class AssetBundleAndroidRequest : AssetBundleRequest
{
	public AssetBundleAndroidRequest(AssetBundleRequestData data, AssetBundle previewBundle = null)
		: base(data, previewBundle)
	{
	}

	public override IEnumerator Send()
	{
		yield return base.Send();
		yield return DownloadCustomManifest();
		if (IsExitedError)
		{
			Initialize();
			yield return DownloadManifest();
			if (IsExitedError)
			{
				yield break;
			}
		}
		yield return DownloadBundle();
		if (!IsExitedError)
		{
			AssetBundleRequest.assetBundles[data.name] = base.Response.bundle;
		}
	}

	private IEnumerator DownloadCustomManifest()
	{
		yield return DownloadManifest(data.customManifestUrl, SetCustomBundleInfoToResponse);
	}

	private void SetCustomBundleInfoToResponse(UnityWebRequest unityWebRequest)
	{
		try
		{
			BundleBuildInfo info = JsonUtility.FromJson<BundleBuildInfo>(unityWebRequest.downloadHandler.text);
			base.Response.info = info;
		}
		catch (Exception exception)
		{
			string message = "Exception on try to parse BundleBuildInfo: " + data.customManifestUrl + "\n";
			HandleException(message, exception);
		}
	}

	protected override bool TrySetBundleFromCache(out CachedAssetBundle cachedAssetBundle)
	{
		bool flag = true;
		if (data.previousBundleInfo.HasValue)
		{
			(BundleBuildInfo, BundleBuildInfo) pair = (data.previousBundleInfo.Value, base.Response.info);
			flag = ((!EqualTimeStamp(pair) && !EqualBundleHash(pair)) ? NotEqualContentHash(pair) : (NotEqualCRC(pair) || NotEqualContentHash(pair)));
		}
		Hash128 hash = (flag ? base.Response.info.bundleHash : data.previousBundleInfo.Value.bundleHash);
		cachedAssetBundle = new CachedAssetBundle(data.name, hash);
		if (!base.Response.isCached || !AssetBundleRequest.assetBundles.TryGetValue(data.name, out var value))
		{
			return false;
		}
		if (((UnityEngine.Object)(object)value).name.Contains(".sd"))
		{
			return false;
		}
		base.Response.bundle = value;
		return true;
	}

	private bool EqualBundleHash((BundleBuildInfo data, BundleBuildInfo response) pair)
	{
		return pair.data.EqualBundleHash(pair.response);
	}

	private bool EqualTimeStamp((BundleBuildInfo data, BundleBuildInfo response) pair)
	{
		return pair.data.EqualTimeStamp(pair.response);
	}

	private bool NotEqualContentHash((BundleBuildInfo data, BundleBuildInfo response) pair)
	{
		return pair.data.NotEqualContentHash(pair.response);
	}

	private bool NotEqualCRC((BundleBuildInfo data, BundleBuildInfo response) pair)
	{
		return pair.data.NotEqualCRC(pair.response);
	}
}
