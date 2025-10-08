using GreenT.HornyScapes;
using UnityEngine;

namespace GreenT.AssetBundles.Communication;

public class AssetBundleResponse : BaseResponse
{
	public BundleBuildInfo info;

	public AssetBundle bundle;
}
