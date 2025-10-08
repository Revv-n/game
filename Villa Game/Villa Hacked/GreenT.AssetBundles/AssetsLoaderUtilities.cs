using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.Settings;
using GreenT.Settings.Data;
using UniRx;
using UnityEngine;

namespace GreenT.AssetBundles;

public static class AssetsLoaderUtilities<T> where T : UnityEngine.Object
{
	public static IObservable<Unit> PreloadAssetByName(IAssetBundlesLoader assetBundlesLoader, Func<string> getPath, string name)
	{
		return Observable.Catch<Unit, Exception>(Observable.ContinueWith<string, Unit>(Observable.Start<string>((Func<string>)(() => getPath()), Scheduler.MainThreadIgnoreTimeScale), (Func<string, IObservable<Unit>>)((string _requestUrl) => assetBundlesLoader.PreloadAsset(_requestUrl))), (Func<Exception, IObservable<Unit>>)delegate(Exception ex)
		{
			throw ex.SendException("Can't load assets:\"" + typeof(T).ToString() + "\" by path:" + getPath());
		});
	}

	public static IObservable<T> LoadAssetByName(IAssetBundlesLoader assetBundlesLoader, Func<string> getPath, string name)
	{
		return Observable.Catch<T, Exception>(Observable.ContinueWith<string, T>(Observable.Start<string>((Func<string>)(() => getPath()), Scheduler.MainThreadIgnoreTimeScale), (Func<string, IObservable<T>>)((string _requestUrl) => assetBundlesLoader.GetAsset<T>(_requestUrl, name))), (Func<Exception, IObservable<T>>)delegate(Exception ex)
		{
			throw ex.SendException("Can't load assets:\"" + typeof(T).ToString() + "\" by path:" + getPath());
		});
	}

	public static IObservable<T> LoadAssetByName(IAssetBundlesLoader assetBundlesLoader, IProjectSettings projectSettings, BundleType bundleType, string name)
	{
		return Observable.Catch<T, Exception>(LoadAssetByName(assetBundlesLoader, () => GetPathByBundleType(projectSettings, bundleType), name), (Func<Exception, IObservable<T>>)delegate(Exception ex)
		{
			throw ex.SendException("Error when loading bundle:\"" + bundleType.ToString() + "\"");
		}).Debug("Load Bundle: " + bundleType, LogType.BundleLoad);
	}

	public static IObservable<IEnumerable<T>> LoadAllAssets(IAssetBundlesLoader assetBundlesLoader, Func<string> getPath)
	{
		return Observable.Catch<IEnumerable<T>, Exception>(Observable.Do<IEnumerable<T>>(Observable.ContinueWith<string, IEnumerable<T>>(Observable.Start<string>((Func<string>)(() => getPath()), Scheduler.MainThreadIgnoreTimeScale), (Func<string, IObservable<IEnumerable<T>>>)assetBundlesLoader.GetAssets<T>), (Action<IEnumerable<T>>)delegate(IEnumerable<T> _ds)
		{
			_ds.Any();
		}), (Func<Exception, IObservable<IEnumerable<T>>>)delegate(Exception ex)
		{
			throw ex.SendException("Can't load assets:\"" + typeof(T)?.ToString() + "\" by path:" + getPath());
		});
	}

	public static IObservable<IEnumerable<T>> LoadAllAssets(IAssetBundlesLoader assetBundlesLoader, IProjectSettings projectSettings, BundleType bundleType)
	{
		return LoadAllAssets(assetBundlesLoader, () => GetPathByBundleType(projectSettings, bundleType));
	}

	public static void ReleaseBundle(IAssetBundlesLoader assetBundlesLoader, string path)
	{
		assetBundlesLoader.Release(path);
	}

	private static string GetPathByBundleType(IProjectSettings projectSettings, BundleType bundleType)
	{
		return projectSettings.BundleUrlResolver.BundleUrl(bundleType);
	}
}
