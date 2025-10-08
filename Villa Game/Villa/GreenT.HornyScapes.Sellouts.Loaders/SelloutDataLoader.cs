using System;
using GreenT.AssetBundles;
using GreenT.HornyScapes.Sellouts.Models;
using GreenT.Settings;
using GreenT.Settings.Data;

namespace GreenT.HornyScapes.Sellouts.Loaders;

public class SelloutDataLoader : AssetBundleLoaderByType<string, SelloutBundleData>
{
	public SelloutDataLoader(IAssetBundlesLoader assetBundlesLoader, IProjectSettings projectSettings, BundleType bundleType)
		: base(assetBundlesLoader, projectSettings, bundleType)
	{
	}

	public override IObservable<SelloutBundleData> Load(string bundleName)
	{
		return base.Load(bundleName);
	}

	public override string GetPath(string bundleName)
	{
		return base.GetPath(bundleName);
	}
}
