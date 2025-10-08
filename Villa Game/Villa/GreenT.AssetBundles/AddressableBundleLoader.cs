using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace GreenT.AssetBundles;

public class AddressableBundleLoader
{
	private readonly Dictionary<Type, AssetLoadStrategy> _loadStrategies = new Dictionary<Type, AssetLoadStrategy> { 
	{
		typeof(Sprite),
		new SpriteAssetLoadStrategy()
	} };

	private readonly GenericAssetLoadStrategy _genericAssetLoadStrategy = new GenericAssetLoadStrategy();

	private readonly AddressableBundleCache _addressableBundleCache;

	private readonly Dictionary<string, BundleOperationHandle> _runningHandles;

	public AddressableBundleLoader(AddressableBundleCache addressableBundleCache)
	{
		_addressableBundleCache = addressableBundleCache;
		_runningHandles = new Dictionary<string, BundleOperationHandle>();
	}

	public IObservable<IAssetBundle> LoadBundle(IList<IResourceLocation> locations, string bundleName)
	{
		return Observable.FromCoroutine((IObserver<IAssetBundle> observer, CancellationToken cancellationToken) => LoadBundleCoroutine(locations, bundleName, observer, cancellationToken));
	}

	private IEnumerator LoadBundleCoroutine(IList<IResourceLocation> locations, string bundleName, IObserver<IAssetBundle> observer, CancellationToken cancellationToken)
	{
		if (_addressableBundleCache.Cache.ContainsKey(bundleName))
		{
			observer.OnNext(_addressableBundleCache.GetBundle(bundleName));
			observer.OnCompleted();
			yield break;
		}
		bool handleCreated = false;
		AsyncOperationHandle<IList<AsyncOperationHandle>> bundleDownloadHandler;
		if (!_runningHandles.TryGetValue(bundleName, out var value))
		{
			List<AsyncOperationHandle> list = new List<AsyncOperationHandle>();
			foreach (IResourceLocation location in locations)
			{
				AssetLoadStrategy value2;
				AsyncOperationHandle item = ((!_loadStrategies.TryGetValue(location.ResourceType, out value2)) ? _genericAssetLoadStrategy.LoadAsset(location) : value2.LoadAsset(location));
				list.Add(item);
			}
			handleCreated = true;
			bundleDownloadHandler = Addressables.ResourceManager.CreateGenericGroupOperation(list);
			_runningHandles.Add(bundleName, new BundleOperationHandle(bundleDownloadHandler));
		}
		else
		{
			bundleDownloadHandler = value.handle;
		}
		while (!bundleDownloadHandler.Task.IsCompleted && !cancellationToken.IsCancellationRequested)
		{
			yield return null;
		}
		if (cancellationToken.IsCancellationRequested)
		{
			if (handleCreated)
			{
				Addressables.Release(bundleDownloadHandler);
			}
			observer.OnCompleted();
			yield break;
		}
		if (bundleDownloadHandler.Task.IsFaulted)
		{
			if (handleCreated)
			{
				Addressables.Release(bundleDownloadHandler);
			}
			observer.OnError(new Exception("Failed to download bundle: " + bundleName));
			yield break;
		}
		AddressableBundle addressableBundle = new AddressableBundle(bundleDownloadHandler.Result.Select((AsyncOperationHandle x) => (UnityEngine.Object)x.Result), bundleDownloadHandler, bundleName);
		if (handleCreated)
		{
			_addressableBundleCache.AddBundle(bundleName, addressableBundle);
			_runningHandles.Remove(bundleName);
		}
		observer.OnNext(addressableBundle);
		observer.OnCompleted();
	}

	public bool TryReleaseBundle(string bundleName)
	{
		bool num = _addressableBundleCache.Cache.ContainsKey(bundleName);
		if (num)
		{
			_addressableBundleCache.Release(bundleName);
		}
		return num;
	}
}
