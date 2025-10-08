using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GreenT.AssetBundles;
using GreenT.Settings;
using GreenT.Settings.Data;
using StripClub.Model.Data;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.MergeCore;

public class MainMergeIconsLoader : ILoader<IEnumerable<Sprite>>
{
	private readonly IAssetBundlesLoader assetBundlesLoader;

	private readonly IProjectSettings projectSettings;

	private readonly BundleType bundleType;

	public MainMergeIconsLoader(IAssetBundlesLoader assetBundlesLoader, IProjectSettings projectSettings, BundleType bundleType)
	{
		this.assetBundlesLoader = assetBundlesLoader;
		this.projectSettings = projectSettings;
		this.bundleType = bundleType;
	}

	public IObservable<IEnumerable<Sprite>> Load()
	{
		return Observable.Select<IAssetBundle, Sprite[]>(Observable.ContinueWith<string, IAssetBundle>(Observable.Start<string>((Func<string>)(() => GetPathByBundleType(projectSettings, bundleType)), Scheduler.MainThread), (Func<string, IObservable<IAssetBundle>>)assetBundlesLoader.DownloadAssetBundle), (Func<IAssetBundle, Sprite[]>)GetObjectsFromAssetBundleSubFolder<Sprite>);
	}

	private T[] GetObjectsFromAssetBundleSubFolder<T>(IAssetBundle bundle) where T : UnityEngine.Object
	{
		string[] array = bundle.GetAllAssetNames().ToArray();
		T[] array2 = new T[array.Length];
		for (int i = 0; i != array.Length; i++)
		{
			array2[i] = bundle.LoadAsset<T>(array[i]);
		}
		return array2.Where((T x) => x != null).ToArray();
	}

	private static string GetPathByBundleType(IProjectSettings projectSettings, BundleType bundleType)
	{
		return Path.Combine(projectSettings.BundleUrlResolver.BundleUrl(bundleType));
	}
}
