using System;
using System.Collections.Generic;
using GreenT.Settings;
using GreenT.Settings.Data;
using StripClub.Model.Data;
using UniRx;
using UnityEngine;

namespace GreenT.AssetBundles;

public class AssetBundleLoader<T> : ILoader<T> where T : UnityEngine.Object
{
	private readonly IAssetBundlesLoader assetBundlesLoader;

	private readonly IProjectSettings projectSettings;

	private readonly BundleType bundleType;

	private readonly string fileName;

	public AssetBundleLoader(IAssetBundlesLoader assetBundlesLoader, IProjectSettings projectSettings, BundleType bundleType, string fileName)
	{
		this.assetBundlesLoader = assetBundlesLoader;
		this.projectSettings = projectSettings;
		this.bundleType = bundleType;
		this.fileName = fileName;
	}

	public virtual IObservable<T> Load()
	{
		return AssetsLoaderUtilities<T>.LoadAssetByName(assetBundlesLoader, projectSettings, bundleType, fileName);
	}
}
public abstract class AssetBundleLoader<K, T> : IBundlesLoader<K, T>, ILoader<K, T> where T : UnityEngine.Object
{
	protected readonly IAssetBundlesLoader assetBundlesLoader;

	public AssetBundleLoader(IAssetBundlesLoader assetBundlesLoader)
	{
		this.assetBundlesLoader = assetBundlesLoader;
	}

	public abstract string GetPath(K bundleName);

	public virtual IObservable<T> Load(K bundleName)
	{
		return Observable.SelectMany<IEnumerable<T>, T>(AssetsLoaderUtilities<T>.LoadAllAssets(assetBundlesLoader, () => GetPath(bundleName)), (Func<IEnumerable<T>, IEnumerable<T>>)((IEnumerable<T> x) => x));
	}

	public virtual IObservable<Unit> Preload(K bundleName)
	{
		string name = bundleName.ToString();
		return AssetsLoaderUtilities<T>.PreloadAssetByName(assetBundlesLoader, () => GetPath(bundleName), name);
	}

	public virtual void ReleaseBundle(K bundleName)
	{
		AssetsLoaderUtilities<T>.ReleaseBundle(assetBundlesLoader, GetPath(bundleName));
	}
}
