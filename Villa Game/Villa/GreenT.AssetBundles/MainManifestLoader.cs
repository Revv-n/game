using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using GreenT.AssetBundles.Communication;
using GreenT.HornyScapes;
using UniRx;
using UnityEngine;

namespace GreenT.AssetBundles;

public class MainManifestLoader
{
	public List<BuildMainInfo> MainManifest;

	private Dictionary<string, IObservable<AssetBundle>> _currentlyRequestingBundles;

	public void BindRequestingBundles(Dictionary<string, IObservable<AssetBundle>> currentlyRequestingBundles)
	{
		_currentlyRequestingBundles = currentlyRequestingBundles;
	}

	public IObservable<List<BuildMainInfo>> DownloadMainManifest(string pathToMainManifest, string version)
	{
		string fileName = GetMainManifestName();
		return Observable.FromCoroutine((IObserver<List<BuildMainInfo>> observer) => DownloadMainManifestRoutine(pathToMainManifest, fileName, version, observer)).DoOnError(delegate(Exception ex)
		{
			throw ex.SendException("Can't load bundle by path: " + pathToMainManifest);
		}).Share();
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
			MainManifest = request.Response.info;
			observer.OnNext(MainManifest);
			observer.OnCompleted();
			_currentlyRequestingBundles.Remove(nameBundle);
		}
		timer.Stop();
	}

	private string GetMainManifestName()
	{
		return "Standalone";
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
}
