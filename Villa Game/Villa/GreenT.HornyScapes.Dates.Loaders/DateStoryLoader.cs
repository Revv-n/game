using GreenT.AssetBundles;
using GreenT.HornyScapes.Dates.Models;
using GreenT.Settings;
using GreenT.Settings.Data;

namespace GreenT.HornyScapes.Dates.Loaders;

public class DateStoryLoader : AssetBundleLoaderByType<(int characterId, int dateId), DateCharacterStories>
{
	public DateStoryLoader(IAssetBundlesLoader assetBundlesLoader, IProjectSettings projectSettings)
		: base(assetBundlesLoader, projectSettings, BundleType.DateStory)
	{
	}

	public override string GetPath((int characterId, int dateId) bundleName)
	{
		return string.Format(projectSettings.BundleUrlResolver.BundleUrl(bundleType), bundleName.characterId, bundleName.dateId);
	}
}
