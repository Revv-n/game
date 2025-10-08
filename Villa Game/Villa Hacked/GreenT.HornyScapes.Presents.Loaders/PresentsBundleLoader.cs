using System;
using GreenT.AssetBundles;
using GreenT.HornyScapes.Presents.Models;
using GreenT.Settings;
using GreenT.Settings.Data;
using UniRx;

namespace GreenT.HornyScapes.Presents.Loaders;

public class PresentsBundleLoader : AssetBundleLoaderByType<string, PresentBundleData>
{
	public PresentsBundleLoader(IAssetBundlesLoader assetBundlesLoader, IProjectSettings projectSettings)
		: base(assetBundlesLoader, projectSettings, BundleType.PresentsIcons)
	{
	}

	public override string GetPath(string bundleName)
	{
		return projectSettings.BundleUrlResolver.BundleUrl(bundleType);
	}

	public override IObservable<PresentBundleData> Load(string bundleName)
	{
		return Observable.Where<PresentBundleData>(base.Load(bundleName), (Func<PresentBundleData, bool>)((PresentBundleData x) => x.name == bundleName.ToString()));
	}
}
