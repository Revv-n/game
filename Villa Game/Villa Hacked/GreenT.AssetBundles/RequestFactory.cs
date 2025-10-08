using System.Collections.Generic;
using GreenT.AssetBundles.Communication;
using GreenT.HornyScapes;
using UnityEngine;

namespace GreenT.AssetBundles;

public class RequestFactory
{
	private Dictionary<string, BundleBuildInfo> _bundlesInfo = new Dictionary<string, BundleBuildInfo>();

	public void BindBundlesInfo(Dictionary<string, BundleBuildInfo> bundlesInfo)
	{
		_bundlesInfo = bundlesInfo;
	}

	public AssetBundleRequest CreateRequest(string path, string nameBundle, BuildMainInfo buildMainInfo, AssetBundle previewAssetBundle = null)
	{
		return new AssetBundleWebGLRequest(new AssetBundleRequestData(nameBundle, path, 3u, buildMainInfo), previewAssetBundle);
	}
}
