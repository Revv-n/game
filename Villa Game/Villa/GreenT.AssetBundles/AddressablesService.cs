using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using GreenT.AssetBundles.Initializators;
using GreenT.Settings;
using GreenT.Settings.Data;
using StripClub.Extensions;
using UniRx;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace GreenT.AssetBundles;

public class AddressablesService : IAddressablesBundlesLoader, IAssetBundlesLoader
{
	private readonly IProjectSettings _projectSettings;

	private readonly AddressableResourceLocationProvider _resourceLocationProvider;

	private readonly AddressableBundleLoader _addressableBundleLoader;

	private readonly AddressableAssetLoader _addressableAssetLoader;

	private readonly IAddressableInitializator _addressableInitializator;

	private readonly IGetConfigUrlParameters _getConfigUrlParameters;

	public static string DownloadURL { get; private set; }

	public AddressablesService(IProjectSettings projectSettings, AddressableResourceLocationProvider resourceLocationProvider, AddressableBundleLoader addressableBundleLoader, AddressableAssetLoader addressableAssetLoader, IGetConfigUrlParameters getConfigUrlParameters)
	{
		_projectSettings = projectSettings;
		_resourceLocationProvider = resourceLocationProvider;
		_addressableBundleLoader = addressableBundleLoader;
		_addressableAssetLoader = addressableAssetLoader;
		_getConfigUrlParameters = getConfigUrlParameters;
		_addressableInitializator = new StandaloneAddressableInitializator();
	}

	public IObservable<Unit> Init()
	{
		DownloadURL = GetPathToCatalogue();
		return _getConfigUrlParameters.Get().SelectMany((ConfigurationInfo version) => Observable.FromCoroutine((CancellationToken _) => _addressableInitializator.Initialize(version.curr_version)));
	}

	public IObservable<Unit> PreloadAsset(string path)
	{
		path = ExtensionMethods.PathCombineUnixStyle(path);
		string assetPath = GetAssetPath(path);
		return _addressableAssetLoader.PreloadAsset(assetPath);
	}

	public IObservable<IAssetBundle> DownloadAssetBundle(string path)
	{
		Stopwatch timer = new Stopwatch();
		timer.Start();
		path = ExtensionMethods.PathCombineUnixStyle(path);
		string assetPath = GetAssetPath(path);
		return _resourceLocationProvider.GetLocations(assetPath).SelectMany((IList<IResourceLocation> locations) => _addressableBundleLoader.LoadBundle(locations, assetPath)).Do(delegate
		{
			LogDownloadTime(timer, assetPath);
		});
	}

	public IObservable<T> GetAsset<T>(string path, string fileName) where T : UnityEngine.Object
	{
		Stopwatch timer = new Stopwatch();
		timer.Start();
		path = ExtensionMethods.PathCombineUnixStyle(path);
		string assetPath = GetAssetPath(path);
		return _addressableAssetLoader.LoadAsset<T>(assetPath).Do(delegate
		{
			LogDownloadTime(timer, assetPath, fileName);
		});
	}

	public IObservable<IEnumerable<T>> GetAssets<T>(string path) where T : UnityEngine.Object
	{
		Stopwatch timer = new Stopwatch();
		timer.Start();
		path = ExtensionMethods.PathCombineUnixStyle(path);
		string groupName = GetAssetPath(path);
		return _resourceLocationProvider.GetLocations(groupName, typeof(T)).SelectMany((Func<IList<IResourceLocation>, IObservable<IEnumerable<T>>>)_addressableAssetLoader.LoadAssets<T>).Do(delegate
		{
			LogDownloadTime(timer, groupName);
		});
	}

	public void Release(string path)
	{
		string assetPath = GetAssetPath(path);
		_addressableAssetLoader.TryReleaseAsset(assetPath);
		_addressableBundleLoader.TryReleaseBundle(assetPath);
	}

	private string GetAssetPath(string path)
	{
		return path.Remove(0, _projectSettings.BundleUrlResolver.BundlesRoot.Length + 1);
	}

	private string GetPathToCatalogue()
	{
		return _projectSettings.BundleUrlResolver.BundlesRoot;
	}

	private void LogDownloadTime(Stopwatch timer, string groupName, string assetName = null)
	{
		timer.Stop();
	}
}
