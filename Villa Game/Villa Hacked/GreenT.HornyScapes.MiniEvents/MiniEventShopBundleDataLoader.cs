using GreenT.AssetBundles;
using GreenT.Settings;
using GreenT.Settings.Data;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class MiniEventShopBundleDataLoader : MiniEventBaseBundleDataLoader
{
	public MiniEventShopBundleDataLoader(IAssetBundlesLoader assetBundlesLoader, IProjectSettings projectSettings)
		: base(assetBundlesLoader, projectSettings, BundleType.MiniEventShop)
	{
	}
}
