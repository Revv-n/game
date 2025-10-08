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
		return Observable.Start(() => getPath(), UniRx.Scheduler.MainThreadIgnoreTimeScale).ContinueWith((string _requestUrl) => assetBundlesLoader.PreloadAsset(_requestUrl)).Catch(delegate(Exception ex)
		{
			throw ex.SendException("Can't load assets:\"" + typeof(T).ToString() + "\" by path:" + getPath());
		});
	}

	public static IObservable<T> LoadAssetByName(IAssetBundlesLoader assetBundlesLoader, Func<string> getPath, string name)
	{
		return Observable.Start(() => getPath(), UniRx.Scheduler.MainThreadIgnoreTimeScale).ContinueWith((string _requestUrl) => assetBundlesLoader.GetAsset<T>(_requestUrl, name)).Catch(delegate(Exception ex)
		{
			throw ex.SendException("Can't load assets:\"" + typeof(T).ToString() + "\" by path:" + getPath());
		});
	}

	public static IObservable<T> LoadAssetByName(IAssetBundlesLoader assetBundlesLoader, IProjectSettings projectSettings, BundleType bundleType, string name)
	{
		return LoadAssetByName(assetBundlesLoader, () => GetPathByBundleType(projectSettings, bundleType), name).Catch(delegate(Exception ex)
		{
			throw ex.SendException("Error when loading bundle:\"" + bundleType.ToString() + "\"");
		}).Debug("Load Bundle: " + bundleType, LogType.BundleLoad);
	}

	public static IObservable<IEnumerable<T>> LoadAllAssets(IAssetBundlesLoader assetBundlesLoader, Func<string> getPath)
	{
		return Observable.Start(() => getPath(), UniRx.Scheduler.MainThreadIgnoreTimeScale).ContinueWith(assetBundlesLoader.GetAssets<T>).Do(delegate(IEnumerable<T> _ds)
		{
			_ds.Any();
		})
			.Catch(delegate(Exception ex)
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
