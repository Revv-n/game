using System;
using System.Collections;
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
		return Observable.SelectMany<ConfigurationInfo, Unit>(_getConfigUrlParameters.Get(), (Func<ConfigurationInfo, IObservable<Unit>>)((ConfigurationInfo version) => Observable.FromCoroutine((Func<CancellationToken, IEnumerator>)((CancellationToken _) => _addressableInitializator.Initialize(version.curr_version)), false)));
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
		return Observable.Do<IAssetBundle>(Observable.SelectMany<IList<IResourceLocation>, IAssetBundle>(_resourceLocationProvider.GetLocations(assetPath), (Func<IList<IResourceLocation>, IObservable<IAssetBundle>>)((IList<IResourceLocation> locations) => _addressableBundleLoader.LoadBundle(locations, assetPath))), (Action<IAssetBundle>)delegate
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
		return Observable.Do<T>(_addressableAssetLoader.LoadAsset<T>(assetPath), (Action<T>)delegate
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
		return Observable.Do<IEnumerable<T>>(Observable.SelectMany<IList<IResourceLocation>, IEnumerable<T>>(_resourceLocationProvider.GetLocations(groupName, typeof(T)), (Func<IList<IResourceLocation>, IObservable<IEnumerable<T>>>)_addressableAssetLoader.LoadAssets<T>), (Action<IEnumerable<T>>)delegate
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
