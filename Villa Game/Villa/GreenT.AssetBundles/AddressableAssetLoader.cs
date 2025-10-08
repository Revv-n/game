using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace GreenT.AssetBundles;

public class AddressableAssetLoader
{
	private readonly AddressableAssetCache _assetCache;

	private readonly Dictionary<string, AsyncOperationHandle> _runningHandles;

	private readonly Dictionary<string, AsyncOperationHandle> _preloadHandles;

	public AddressableAssetLoader(AddressableAssetCache assetCache)
	{
		_assetCache = assetCache;
		_runningHandles = new Dictionary<string, AsyncOperationHandle>();
		_preloadHandles = new Dictionary<string, AsyncOperationHandle>();
	}

	public IObservable<Unit> PreloadAsset(string assetName)
	{
		return Observable.FromCoroutine((IObserver<Unit> observer, CancellationToken cancellationToken) => PreloadAssetCoroutine(assetName, observer, cancellationToken));
	}

	public IObservable<TAsset> LoadAsset<TAsset>(IResourceLocation location) where TAsset : UnityEngine.Object
	{
		return Observable.FromCoroutine((IObserver<TAsset> observer, CancellationToken cancellationToken) => LoadAssetCoroutine(location, observer, cancellationToken));
	}

	public IObservable<TAsset> LoadAsset<TAsset>(string assetPath) where TAsset : UnityEngine.Object
	{
		return Observable.FromCoroutine((IObserver<TAsset> observer, CancellationToken cancellationToken) => LoadAssetCoroutine(assetPath, observer, cancellationToken));
	}

	public IObservable<IEnumerable<TAsset>> LoadAssets<TAsset>(IEnumerable<IResourceLocation> locations) where TAsset : UnityEngine.Object
	{
		return locations.ToObservable().SelectMany((Func<IResourceLocation, IObservable<TAsset>>)LoadAsset<TAsset>).ToList();
	}

	private IEnumerator LoadAssetCoroutine<TAsset>(IResourceLocation resourceLocation, IObserver<TAsset> observer, CancellationToken cancellationToken) where TAsset : UnityEngine.Object
	{
		if (_assetCache.HasAsset(resourceLocation.InternalId))
		{
			observer.OnNext(_assetCache.GetAsset<TAsset>(resourceLocation.InternalId));
			observer.OnCompleted();
			yield break;
		}
		bool handleCreated = false;
		AsyncOperationHandle handle;
		if (_runningHandles.TryGetValue(resourceLocation.InternalId, out var value))
		{
			handle = value;
		}
		else
		{
			handle = Addressables.LoadAssetAsync<TAsset>(resourceLocation);
			_runningHandles.Add(resourceLocation.InternalId, handle);
			handleCreated = true;
		}
		while (!handle.Task.IsCompleted && !cancellationToken.IsCancellationRequested)
		{
			yield return null;
		}
		if (cancellationToken.IsCancellationRequested)
		{
			if (handleCreated)
			{
				Addressables.Release(handle);
			}
			observer.OnCompleted();
			yield break;
		}
		if (handle.Task.IsFaulted)
		{
			if (handleCreated)
			{
				Addressables.Release(handle);
			}
			_runningHandles.Remove(resourceLocation.InternalId);
			observer.OnError(new Exception("Failed to load asset: " + resourceLocation.InternalId));
			yield break;
		}
		if (handleCreated)
		{
			_assetCache.Add(resourceLocation.InternalId, handle);
			_runningHandles.Remove(resourceLocation.InternalId);
		}
		observer.OnNext(handle.Result as TAsset);
		observer.OnCompleted();
	}

	private IEnumerator LoadAssetCoroutine<TAsset>(string assetPath, IObserver<TAsset> observer, CancellationToken cancellationToken) where TAsset : UnityEngine.Object
	{
		if (_assetCache.HasAsset(assetPath))
		{
			observer.OnNext(_assetCache.GetAsset<TAsset>(assetPath));
			observer.OnCompleted();
			yield break;
		}
		bool handleCreated = false;
		AsyncOperationHandle handle;
		if (_runningHandles.TryGetValue(assetPath, out var value))
		{
			handle = value;
		}
		else
		{
			handle = Addressables.LoadAssetAsync<TAsset>(assetPath);
			_runningHandles.Add(assetPath, handle);
			handleCreated = true;
		}
		while (!handle.Task.IsCompleted && !cancellationToken.IsCancellationRequested)
		{
			yield return null;
		}
		if (cancellationToken.IsCancellationRequested)
		{
			if (handleCreated)
			{
				Addressables.Release(handle);
			}
			observer.OnCompleted();
			yield break;
		}
		if (handle.Task.IsFaulted)
		{
			if (handleCreated)
			{
				Addressables.Release(handle);
			}
			_runningHandles.Remove(assetPath);
			observer.OnError(new Exception("Failed to load asset: " + assetPath));
			yield break;
		}
		if (handleCreated)
		{
			_assetCache.Add(assetPath, handle);
			_runningHandles.Remove(assetPath);
		}
		observer.OnNext(handle.Result as TAsset);
		observer.OnCompleted();
	}

	private IEnumerator PreloadAssetCoroutine(string assetPath, IObserver<Unit> observer, CancellationToken cancellationToken)
	{
		if (_assetCache.HasPreloadAsset(assetPath))
		{
			observer.OnCompleted();
			yield break;
		}
		bool handleCreated = false;
		AsyncOperationHandle handle;
		if (_preloadHandles.TryGetValue(assetPath, out var value))
		{
			handle = value;
		}
		else
		{
			handle = Addressables.DownloadDependenciesAsync(assetPath);
			_preloadHandles.Add(assetPath, handle);
			handleCreated = true;
		}
		while (!handle.Task.IsCompleted && !cancellationToken.IsCancellationRequested)
		{
			yield return null;
		}
		if (cancellationToken.IsCancellationRequested)
		{
			if (handleCreated)
			{
				Addressables.Release(handle);
			}
			observer.OnCompleted();
			yield break;
		}
		if (handle.Task.IsFaulted)
		{
			if (handleCreated)
			{
				Addressables.Release(handle);
			}
			_preloadHandles.Remove(assetPath);
			observer.OnError(new Exception("Failed to load asset: " + assetPath));
			yield break;
		}
		if (handleCreated)
		{
			_assetCache.AddPreload(assetPath, handle);
			_preloadHandles.Remove(assetPath);
		}
		observer.OnNext(default(Unit));
		observer.OnCompleted();
	}

	public bool TryReleaseAsset(string assetPath)
	{
		if (_runningHandles.TryGetValue(assetPath, out var value))
		{
			Addressables.Release(value);
			_runningHandles.Remove(assetPath);
			return true;
		}
		bool num = _assetCache.HasAsset(assetPath);
		if (num)
		{
			_assetCache.Release(assetPath);
		}
		return num;
	}
}
