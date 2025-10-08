using GreenT.AssetBundles;
using GreenT.Settings;
using GreenT.Settings.Data;

namespace GreenT.HornyScapes.Events;

public class EventBundleDataLoader : AssetBundleLoaderByType<string, EventBundleData>
{
	public EventBundleDataLoader(IAssetBundlesLoader assetBundlesLoader, IProjectSettings projectSettings, BundleType bundleType)
		: base(assetBundlesLoader, projectSettings, bundleType)
	{
	}
}
