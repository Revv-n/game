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

namespace GreenT.HornyScapes.MiniEvents;

public class MiniEventMergeIconsLoader : IBundlesLoader<string, IEnumerable<Sprite>>, ILoader<string, IEnumerable<Sprite>>
{
	protected readonly IAssetBundlesLoader assetBundlesLoader;

	protected readonly IProjectSettings projectSettings;

	protected readonly BundleType bundleType;

	private string subFolderName;

	public MiniEventMergeIconsLoader(IAssetBundlesLoader assetBundlesLoader, IProjectSettings projectSettings, BundleType bundleType, string subFolderName)
	{
		this.assetBundlesLoader = assetBundlesLoader;
		this.projectSettings = projectSettings;
		this.bundleType = bundleType;
		this.subFolderName = subFolderName.ToLower();
	}

	public IObservable<IEnumerable<Sprite>> Load(string bundleName)
	{
		return from _bundle in Observable.Start(() => GetPathByBundleType(projectSettings, bundleType, bundleName), Scheduler.MainThread).ContinueWith(assetBundlesLoader.DownloadAssetBundle)
			select GetObjectsFromAssetBundleSubFolder<Sprite>(_bundle, subFolderName);
	}

	public void ReleaseBundle(string bundleName)
	{
		string pathByBundleType = GetPathByBundleType(projectSettings, bundleType, bundleName);
		assetBundlesLoader.Release(pathByBundleType);
	}

	private T[] GetObjectsFromAssetBundleSubFolder<T>(IAssetBundle bundle, string subFolderName) where T : UnityEngine.Object
	{
		string[] array = (from _fullPath in bundle.GetAllAssetNames()
			where _fullPath.Contains(subFolderName)
			select _fullPath).ToArray();
		T[] array2 = new T[array.Length];
		for (int i = 0; i != array.Length; i++)
		{
			array2[i] = bundle.LoadAsset<T>(array[i]);
		}
		return array2;
	}

	private static string GetPathByBundleType(IProjectSettings projectSettings, BundleType bundleType, string bundleName)
	{
		return Path.Combine(projectSettings.BundleUrlResolver.BundleUrl(bundleType), bundleName.ToString().ToLower());
	}
}
