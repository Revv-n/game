using System;
using System.Collections.Generic;
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
		return Observable.AsUnitObservable<IAssetBundle>(Observable.Do<IAssetBundle>(Observable.ObserveOn<IAssetBundle>(assetBundlesManager.DownloadAssetBundle(path), Scheduler.MainThreadIgnoreTimeScale), (Action<IAssetBundle>)delegate(IAssetBundle bundle)
		{
			_bundlesProvider.TryAdd(contentSource, bundle);
		}));
	}

	public IObservable<T> Load<T>(BundleType bundleType, string bundleName, ContentSource contentSource) where T : UnityEngine.Object
	{
		string path = GetPath(bundleType, bundleName);
		return Observable.SelectMany<T[], T>(Observable.DoWhere<T[]>(Observable.Select<IAssetBundle, T[]>(Observable.Do<IAssetBundle>(Observable.ObserveOn<IAssetBundle>(assetBundlesManager.DownloadAssetBundle(path), Scheduler.MainThreadIgnoreTimeScale), (Action<IAssetBundle>)delegate(IAssetBundle bundle)
		{
			_bundlesProvider.TryAdd(contentSource, bundle);
		}), (Func<IAssetBundle, T[]>)((IAssetBundle bundle) => bundle.LoadAllAssets<T>())), (Action<T[]>)delegate
		{
		}, (Func<T[], bool>)((T[] _ds) => _ds.Length == 0)), (Func<T[], IEnumerable<T>>)((T[] asset) => asset));
	}

	public IObservable<T> Load<T>(BundleType bundleType, string bundleName) where T : UnityEngine.Object
	{
		string path = GetPath(bundleType, bundleName);
		return Observable.SelectMany<T[], T>(Observable.DoWhere<T[]>(Observable.Select<IAssetBundle, T[]>(Observable.ObserveOn<IAssetBundle>(assetBundlesManager.DownloadAssetBundle(path), Scheduler.MainThreadIgnoreTimeScale), (Func<IAssetBundle, T[]>)((IAssetBundle bundle) => bundle.LoadAllAssets<T>())), (Action<T[]>)delegate
		{
		}, (Func<T[], bool>)((T[] _ds) => _ds.Length == 0)), (Func<T[], IEnumerable<T>>)((T[] asset) => asset));
	}

	private string GetPath(BundleType bundleType, string bundleName)
	{
		string text = projectSettings.BundleUrlResolver.BundleUrl(bundleType);
		return ExtensionMethods.PathCombineUnixStyle(text, bundleName);
	}
}
