using System;
using System.IO;
using GreenT.Settings;
using GreenT.Settings.Data;
using UniRx;
using UnityEngine;

namespace GreenT.AssetBundles;

public class AssetBundleLoaderByType<K, T> : AssetBundleLoader<K, T> where T : UnityEngine.Object
{
	protected readonly IProjectSettings projectSettings;

	protected readonly BundleType bundleType;

	public AssetBundleLoaderByType(IAssetBundlesLoader assetBundlesLoader, IProjectSettings projectSettings, BundleType bundleType)
		: base(assetBundlesLoader)
	{
		this.projectSettings = projectSettings;
		this.bundleType = bundleType;
	}

	public override string GetPath(K bundleName)
	{
		return Path.Combine(projectSettings.BundleUrlResolver.BundleUrl(bundleType), bundleName.ToString().ToLower()).Replace('\\', '/');
	}

	public override IObservable<T> Load(K bundleName)
	{
		return base.Load(bundleName).Catch(delegate(InvalidCastException exception)
		{
			throw exception.SendException("Error when loading bundle:\"" + bundleType.ToString() + "\"");
		}).Debug("Load Bundle: " + bundleType, LogType.BundleLoad);
	}
}
