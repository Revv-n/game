using System;
using System.Linq;
using GreenT.AssetBundles;
using GreenT.Settings;
using GreenT.Settings.Data;
using StripClub.Model.Data;
using UniRx;

namespace GreenT.HornyScapes.BannerSpace;

public class BackgroundBundleLoader : ILoader<string, BannerBackgroundBundle>
{
	private readonly IAssetBundlesLoader _assetBundlesManager;

	private readonly IProjectSettings _projectSettings;

	private const BundleType BundleType = BundleType.BannerBackground;

	public BackgroundBundleLoader(IAssetBundlesLoader assetBundlesManager, IProjectSettings projectSettings)
	{
		_assetBundlesManager = assetBundlesManager;
		_projectSettings = projectSettings;
	}

	public IObservable<BannerBackgroundBundle> Load(string bundleName)
	{
		return from assetBundle in _assetBundlesManager.DownloadAssetBundle(GetPath(bundleName))
			select assetBundle.LoadAllAssets<BannerBackgroundBundle>().FirstOrDefault();
	}

	private string GetPath(string bundleName)
	{
		return _projectSettings.BundleUrlResolver.BundleUrl(BundleType.BannerBackground) + "/" + bundleName.ToLower();
	}
}
