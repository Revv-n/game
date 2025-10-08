using GreenT.AssetBundles;
using GreenT.HornyScapes.Dates.Models;
using GreenT.Settings;
using GreenT.Settings.Data;

namespace GreenT.HornyScapes.Dates.Loaders;

public class DateBackgroundDataLoader : AssetBundleLoaderByType<string, DateBackgroundData>
{
	public DateBackgroundDataLoader(IAssetBundlesLoader assetBundlesLoader, IProjectSettings projectSettings)
		: base(assetBundlesLoader, projectSettings, BundleType.DateBackground)
	{
	}

	public override string GetPath(string bundleName)
	{
		return string.Format(projectSettings.BundleUrlResolver.BundleUrl(bundleType), bundleName);
	}
}
