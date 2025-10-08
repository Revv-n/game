using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace GreenT.AssetBundles;

public class AddressableResourceLocationProvider
{
	public IObservable<IList<IResourceLocation>> GetLocations(string groupName)
	{
		AsyncOperationHandle<IList<IResourceLocation>> locationsHandler = Addressables.LoadResourceLocationsAsync(groupName);
		return Observable.FromCoroutine<IList<IResourceLocation>>((Func<IObserver<IList<IResourceLocation>>, CancellationToken, IEnumerator>)((IObserver<IList<IResourceLocation>> observer, CancellationToken cancellationToken) => GetLocationsCoroutine(locationsHandler, observer, cancellationToken)));
	}

	public IObservable<IList<IResourceLocation>> GetLocations(string groupName, Type type)
	{
		AsyncOperationHandle<IList<IResourceLocation>> locationsHandler = Addressables.LoadResourceLocationsAsync(groupName, type);
		return Observable.FromCoroutine<IList<IResourceLocation>>((Func<IObserver<IList<IResourceLocation>>, CancellationToken, IEnumerator>)((IObserver<IList<IResourceLocation>> observer, CancellationToken cancellationToken) => GetLocationsCoroutine(locationsHandler, observer, cancellationToken)));
	}

	private IEnumerator GetLocationsCoroutine(AsyncOperationHandle<IList<IResourceLocation>> locationsHandler, IObserver<IList<IResourceLocation>> observer, CancellationToken cancellationToken)
	{
		while (!locationsHandler.Task.IsCompleted && !cancellationToken.IsCancellationRequested)
		{
			yield return null;
		}
		if (cancellationToken.IsCancellationRequested)
		{
			observer.OnCompleted();
			Addressables.Release(locationsHandler);
		}
		else if (locationsHandler.Task.IsFaulted)
		{
			observer.OnError(new Exception("Failed to provide resource locations for: " + locationsHandler.DebugName));
		}
		else
		{
			observer.OnNext(locationsHandler.Result);
			observer.OnCompleted();
			Addressables.Release(locationsHandler);
		}
	}
}
