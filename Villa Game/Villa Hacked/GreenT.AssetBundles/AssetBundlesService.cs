using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GreenT.AssetBundles.Communication;
using GreenT.HornyScapes;
using GreenT.Settings;
using GreenT.Settings.Data;
using ModestTree;
using StripClub.Extensions;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.AssetBundles;

public class AssetBundlesService : MonoBehaviour, IAssetBundlesLoader, IAssetBundlesCache
{
	private IProjectSettings _projectSettings;

	private IGetConfigUrlParameters _getConfigUrlParameters;

	private List<BuildMainInfo> _mainManifest;

	private readonly Dictionary<string, IAssetBundle> _assetBundles = new Dictionary<string, IAssetBundle>();

	private readonly Dictionary<string, IObservable<IAssetBundle>> _currentlyRequestingBundles = new Dictionary<string, IObservable<IAssetBundle>>();

	private readonly Dictionary<string, BundleBuildInfo> bundlesInfo = new Dictionary<string, BundleBuildInfo>();

	public IDictionary<string, IAssetBundle> Cache => _assetBundles;

	[Inject]
	private void Init(IProjectSettings projectSettings, IGetConfigUrlParameters getConfigUrlParameters)
	{
		_getConfigUrlParameters = getConfigUrlParameters;
		_projectSettings = projectSettings;
		AssetBundleRequest.assetBundles.Clear();
	}

	public IAssetBundle GetBundle(string bundleName)
	{
		Cache.TryGetValue(bundleName, out var value);
		return value;
	}

	public IObservable<Unit> Init()
	{
		return Observable.Select<List<BuildMainInfo>, Unit>(Observable.SelectMany<ConfigurationInfo, List<BuildMainInfo>>(_getConfigUrlParameters.Get(), (Func<ConfigurationInfo, IObservable<List<BuildMainInfo>>>)((ConfigurationInfo version) => DownloadMainManifest(PathToMainManifest(), version.curr_version))), (Func<List<BuildMainInfo>, Unit>)((List<BuildMainInfo> _) => Unit.Default));
		string PathToMainManifest()
		{
			return _projectSettings.BundleUrlResolver.BundleUrl(BundleType.MainManifest);
		}
	}

	private IObservable<List<BuildMainInfo>> DownloadMainManifest(string pathToMainManifest, string version)
	{
		string fileName = GetMainManifestName();
		return Observable.Share<List<BuildMainInfo>>(Observable.DoOnError<List<BuildMainInfo>>(Observable.FromCoroutine<List<BuildMainInfo>>((Func<IObserver<List<BuildMainInfo>>, IEnumerator>)((IObserver<List<BuildMainInfo>> observer) => DownloadMainManifestRoutine(pathToMainManifest, fileName, version, observer))), (Action<Exception>)delegate(Exception ex)
		{
			throw ex.SendException("Can't load bundle by path: " + pathToMainManifest);
		}));
	}

	private string GetMainManifestName()
	{
		return "Standalone";
	}

	private IEnumerator DownloadMainManifestRoutine(string path, string nameBundle, string version, IObserver<List<BuildMainInfo>> observer)
	{
		Stopwatch timer = new Stopwatch();
		timer.Start();
		MainManifestRequestData data = new MainManifestRequestData(nameBundle, path, 3u, version);
		MainManifestRequest request = new MainManifestRequest(data);
		yield return request.Send();
		if (request.IsExitedError)
		{
			HandleException(nameBundle, observer, request);
		}
		else
		{
			_mainManifest = request.Response.info;
			observer.OnNext(_mainManifest);
			observer.OnCompleted();
			_currentlyRequestingBundles.Remove(nameBundle);
		}
		timer.Stop();
	}

	private void HandleException(string nameBundle, IObserver<List<BuildMainInfo>> observer, MainManifestRequest request)
	{
		if (request.HasError)
		{
			observer.OnError(request.Error);
			return;
		}
		ArgumentNullException error = new ArgumentNullException("Unknown error occurred while loading asset bundle: " + nameBundle);
		observer.OnError(error);
	}

	public IObservable<Unit> PreloadAsset(string path)
	{
		throw new NotImplementedException();
	}

	public IObservable<IAssetBundle> DownloadAssetBundle(string path)
	{
		Assert.IsNotNull((object)path);
		path = ExtensionMethods.PathCombineUnixStyle(path);
		string nameBundle = GetNameBundle(path);
		if (_assetBundles.ContainsKey(nameBundle))
		{
			return Observable.Return<IAssetBundle>(_assetBundles[nameBundle]);
		}
		if (_currentlyRequestingBundles.ContainsKey(nameBundle))
		{
			return _currentlyRequestingBundles[nameBundle];
		}
		IObservable<IAssetBundle> observable = Observable.Share<IAssetBundle>(Observable.DoOnError<IAssetBundle>(Observable.FromCoroutine<IAssetBundle>((Func<IObserver<IAssetBundle>, IEnumerator>)((IObserver<IAssetBundle> observer) => DownloadAssetBundleRoutine(path, nameBundle, observer))), (Action<Exception>)delegate(Exception ex)
		{
			throw ex.SendException("Can't load bundle by path: " + path);
		}));
		_currentlyRequestingBundles.Add(nameBundle, observable);
		return observable;
	}

	private IEnumerator DownloadAssetBundleRoutine(string path, string nameBundle, IObserver<IAssetBundle> observer)
	{
		Stopwatch timer = new Stopwatch();
		timer.Start();
		BuildMainInfo buildMainInfo = _mainManifest.First((BuildMainInfo _file) => _file.path == nameBundle);
		AssetBundleRequest request = CreateRequest(path, nameBundle, buildMainInfo);
		yield return request.Send();
		if (request.IsExitedError)
		{
			HandleException(nameBundle, observer, request);
		}
		else
		{
			_assetBundles[nameBundle] = new AssetBundleWrapper(request.Response.bundle);
			observer.OnNext(_assetBundles[nameBundle]);
			observer.OnCompleted();
			_currentlyRequestingBundles.Remove(nameBundle);
		}
		timer.Stop();
		ReportMessage(nameBundle, request, timer);
	}

	private AssetBundleRequest CreateRequest(string path, string nameBundle, BuildMainInfo buildMainInfo)
	{
		return new AssetBundleWebGLRequest(new AssetBundleRequestData(nameBundle, path, 3u, buildMainInfo));
	}

	private void HandleException(string nameBundle, IObserver<IAssetBundle> observer, AssetBundleRequest request)
	{
		if (_assetBundles.ContainsKey(nameBundle))
		{
			observer.OnNext(_assetBundles[nameBundle]);
			observer.OnCompleted();
		}
		else if (request.HasError)
		{
			if (request.Error.Message == "Can't be loaded because another AssetBundle with the same files is already loaded.")
			{
				if (_assetBundles.ContainsKey(nameBundle))
				{
					observer.OnNext(_assetBundles[nameBundle]);
					observer.OnCompleted();
				}
				else
				{
					observer.OnError(request.Error);
				}
			}
			else
			{
				observer.OnError(request.Error);
			}
		}
		else
		{
			ArgumentNullException error = new ArgumentNullException("Unknown error occurred while loading asset bundle: " + nameBundle);
			observer?.OnError(error);
		}
	}

	private void ReportMessage(string nameBundle, AssetBundleRequest request, Stopwatch timer)
	{
		AssetBundleReporter.SetReportAssetBundle(new AssetBundleReport(request.Response.info.name, request.BundleUrl, request.Response.isCached, request.Response.info.bundleHash, timer.Elapsed));
	}

	private string GetNameBundle(string path)
	{
		return path.Remove(0, _projectSettings.BundleUrlResolver.BundlesRoot.Length + 1);
	}

	public IObservable<UnityEngine.Object> GetAsset(string path, string fileName, Type type)
	{
		return Observable.Catch<UnityEngine.Object, Exception>(Observable.Select<IAssetBundle, UnityEngine.Object>(DownloadAssetBundle(path), (Func<IAssetBundle, UnityEngine.Object>)((IAssetBundle bundle) => bundle.LoadAsset(fileName, type))), (Func<Exception, IObservable<UnityEngine.Object>>)delegate(Exception ex)
		{
			throw ex.SendException("Can't load asset:\"" + fileName + "\" by path:" + path);
		});
	}

	public IObservable<T> GetAsset<T>(string path, string fileName) where T : UnityEngine.Object
	{
		return Observable.Cast<UnityEngine.Object, T>(GetAsset(path, fileName, typeof(T)));
	}

	public IObservable<IEnumerable<T>> GetAssets<T>(string path) where T : UnityEngine.Object
	{
		return Observable.Select<IAssetBundle, T[]>(DownloadAssetBundle(path), (Func<IAssetBundle, T[]>)((IAssetBundle bundle) => bundle.LoadAllAssets<T>()));
	}

	public void ReleaseAll()
	{
		foreach (KeyValuePair<string, IAssetBundle> item in Cache)
		{
			item.Value.Unload(b: true);
		}
	}

	public void Release(string path)
	{
		string nameBundle = GetNameBundle(path);
		if (Cache.TryGetValue(nameBundle, out var value))
		{
			value.Unload(b: true);
			Cache.Remove(nameBundle);
			AssetBundleRequest.assetBundles.Remove(nameBundle);
		}
	}
}
