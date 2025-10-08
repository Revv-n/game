using System;
using System.Collections.Generic;
using GreenT.Settings;
using GreenT.Settings.Data;
using StripClub.Model.Data;
using UnityEngine;

namespace GreenT.AssetBundles;

public class AssetsLoaderFromAssetBundle<T> : ILoader<IEnumerable<T>> where T : UnityEngine.Object
{
	protected readonly IAssetBundlesLoader assetBundlesLoader;

	private readonly IProjectSettings projectSettings;

	private readonly BundleType bundleType;

	public AssetsLoaderFromAssetBundle(IAssetBundlesLoader assetBundlesLoader, IProjectSettings projectSettings, BundleType bundleType)
	{
		this.assetBundlesLoader = assetBundlesLoader;
		this.projectSettings = projectSettings;
		this.bundleType = bundleType;
	}

	public virtual IObservable<IEnumerable<T>> Load()
	{
		return AssetsLoaderUtilities<T>.LoadAllAssets(assetBundlesLoader, projectSettings, bundleType);
	}
}
