using GreenT.AssetBundles;
using GreenT.HornyScapes.MiniEvents;
using GreenT.Settings;
using GreenT.Settings.Data;

namespace GreenT.HornyScapes;

public sealed class MiniEventBundleDataLoader : MiniEventBaseBundleDataLoader
{
	public MiniEventBundleDataLoader(IAssetBundlesLoader assetBundlesLoader, IProjectSettings projectSettings)
		: base(assetBundlesLoader, projectSettings, BundleType.MiniEvent)
	{
	}
}
