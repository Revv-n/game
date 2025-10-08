using GreenT.AssetBundles;
using GreenT.Settings;
using GreenT.Settings.Data;

namespace GreenT.HornyScapes.Events;

public class BattlePassBundleDataLoader : AssetBundleLoaderByType<string, BattlePassBundleData>
{
	public BattlePassBundleDataLoader(IAssetBundlesLoader assetBundlesLoader, IProjectSettings projectSettings, BundleType bundleType)
		: base(assetBundlesLoader, projectSettings, bundleType)
	{
	}
}
