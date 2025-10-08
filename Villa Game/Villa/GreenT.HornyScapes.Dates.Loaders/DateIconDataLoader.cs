using GreenT.AssetBundles;
using GreenT.HornyScapes.Dates.Models;
using GreenT.Settings;
using GreenT.Settings.Data;

namespace GreenT.HornyScapes.Dates.Loaders;

public class DateIconDataLoader : AssetBundleLoaderByType<int, DateIconData>
{
	public DateIconDataLoader(IAssetBundlesLoader assetBundlesLoader, IProjectSettings projectSettings)
		: base(assetBundlesLoader, projectSettings, BundleType.DateIcon)
	{
	}

	public override string GetPath(int bundleName)
	{
		return string.Format(projectSettings.BundleUrlResolver.BundleUrl(bundleType), bundleName);
	}
}
