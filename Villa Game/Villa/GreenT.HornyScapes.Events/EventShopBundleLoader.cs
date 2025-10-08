using System;
using GreenT.AssetBundles;
using GreenT.Settings;
using GreenT.Settings.Data;
using StripClub.Model.Data;

namespace GreenT.HornyScapes.Events;

public class EventShopBundleLoader : IBundlesLoader<string, IAssetBundle>, ILoader<string, IAssetBundle>
{
	private readonly IAssetBundlesLoader assetBundlesManager;

	private readonly IProjectSettings projectSettings;

	private readonly BundleType bundleType = BundleType.EventShop;

	public EventShopBundleLoader(IAssetBundlesLoader assetBundlesManager, IProjectSettings projectSettings)
	{
		this.assetBundlesManager = assetBundlesManager;
		this.projectSettings = projectSettings;
	}

	public IObservable<IAssetBundle> Load(string bundleName)
	{
		string path = GetPath(bundleName);
		return assetBundlesManager.DownloadAssetBundle(path);
	}

	public void ReleaseBundle(string bundleName)
	{
		string path = GetPath(bundleName);
		assetBundlesManager.Release(path);
	}

	private string GetPath(string bundleName)
	{
		return projectSettings.BundleUrlResolver.BundleUrl(bundleType) + bundleName.ToLower();
	}
}
