using System;
using GreenT.HornyScapes;
using GreenT.Settings;
using GreenT.Settings.Data;
using StripClub.Extensions;
using StripClub.Model.Shop.Data;
using UniRx;
using UnityEngine;

namespace GreenT.AssetBundles;

public class BundleLoader
{
	private readonly IAssetBundlesLoader assetBundlesManager;

	private readonly IProjectSettings projectSettings;

	private readonly BundlesProviderBase _bundlesProvider;

	public BundleLoader(IAssetBundlesLoader assetBundlesManager, IProjectSettings projectSettings, BundlesProviderBase bundlesProvider)
	{
		this.assetBundlesManager = assetBundlesManager;
		this.projectSettings = projectSettings;
		_bundlesProvider = bundlesProvider;
	}

	public IObservable<Unit> LoadBundle(BundleType bundleType, string bundleName, ContentSource contentSource)
	{
		string path = GetPath(bundleType, bundleName);
		return assetBundlesManager.DownloadAssetBundle(path).ObserveOn(UniRx.Scheduler.MainThreadIgnoreTimeScale).Do(delegate(IAssetBundle bundle)
		{
			_bundlesProvider.TryAdd(contentSource, bundle);
		})
			.AsUnitObservable();
	}

	public IObservable<T> Load<T>(BundleType bundleType, string bundleName, ContentSource contentSource) where T : UnityEngine.Object
	{
		string path = GetPath(bundleType, bundleName);
		return (from bundle in assetBundlesManager.DownloadAssetBundle(path).ObserveOn(UniRx.Scheduler.MainThreadIgnoreTimeScale).Do(delegate(IAssetBundle bundle)
			{
				_bundlesProvider.TryAdd(contentSource, bundle);
			})
			select bundle.LoadAllAssets<T>()).DoWhere(delegate
		{
		}, (T[] _ds) => _ds.Length == 0).SelectMany((T[] asset) => asset);
	}

	public IObservable<T> Load<T>(BundleType bundleType, string bundleName) where T : UnityEngine.Object
	{
		string path = GetPath(bundleType, bundleName);
		return (from bundle in assetBundlesManager.DownloadAssetBundle(path).ObserveOn(UniRx.Scheduler.MainThreadIgnoreTimeScale)
			select bundle.LoadAllAssets<T>()).DoWhere(delegate
		{
		}, (T[] _ds) => _ds.Length == 0).SelectMany((T[] asset) => asset);
	}

	private string GetPath(BundleType bundleType, string bundleName)
	{
		string text = projectSettings.BundleUrlResolver.BundleUrl(bundleType);
		return ExtensionMethods.PathCombineUnixStyle(text, bundleName);
	}
}
