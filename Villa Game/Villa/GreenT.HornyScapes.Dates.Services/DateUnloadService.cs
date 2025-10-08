using System.Linq;
using GreenT.AssetBundles;
using GreenT.HornyScapes.Dates.Models;
using GreenT.Settings;
using GreenT.Settings.Data;

namespace GreenT.HornyScapes.Dates.Services;

public class DateUnloadService
{
	private readonly IAssetBundlesCache _assetBundlesCache;

	private readonly IProjectSettings _projectSetting;

	public DateUnloadService(IAssetBundlesCache assetBundlesCache, IProjectSettings projectSetting)
	{
		_assetBundlesCache = assetBundlesCache;
		_projectSetting = projectSetting;
	}

	public void Unload(Date date)
	{
		date.Steps.Select((DatePhrase x) => x.BackgroundDatas);
		date.Steps.Select((DatePhrase x) => x.CharacterID).Distinct();
	}

	private string GetBackgroundBundleName(string id)
	{
		return GetBundleName(id, BundleType.DateBackground);
	}

	private string GetStoryBundleName(int id)
	{
		return GetBundleName(id.ToString(), BundleType.DateStory);
	}

	private string GetBundleName(string name, BundleType bundleType)
	{
		return string.Format(_projectSetting.BundleUrlResolver.BundleUrl(bundleType), name).Remove(0, _projectSetting.BundleUrlResolver.BundlesRoot.Length + 1);
	}
}
