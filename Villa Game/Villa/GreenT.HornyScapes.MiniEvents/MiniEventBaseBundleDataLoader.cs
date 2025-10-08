using System;
using GreenT.AssetBundles;
using GreenT.Settings;
using GreenT.Settings.Data;
using StripClub.Model.Data;

namespace GreenT.HornyScapes.MiniEvents;

public abstract class MiniEventBaseBundleDataLoader : IBundlesLoader<string, IAssetBundle>, ILoader<string, IAssetBundle>
{
	private readonly IAssetBundlesLoader _assetBundlesLoader;

	private readonly IProjectSettings _projectSettings;

	private readonly BundleType _bundleType;

	protected MiniEventBaseBundleDataLoader(IAssetBundlesLoader assetBundlesLoader, IProjectSettings projectSettings, BundleType bundleType)
	{
		_assetBundlesLoader = assetBundlesLoader;
		_projectSettings = projectSettings;
		_bundleType = bundleType;
	}

	public IObservable<IAssetBundle> Load(string bundleName)
	{
		string path = GetPath(bundleName);
		return _assetBundlesLoader.DownloadAssetBundle(path);
	}

	public void ReleaseBundle(string bundleName)
	{
		string path = GetPath(bundleName);
		_assetBundlesLoader.Release(path);
	}

	private string GetPath(string bundleName)
	{
		return _projectSettings.BundleUrlResolver.BundleUrl(_bundleType) + bundleName.ToLower();
	}
}
