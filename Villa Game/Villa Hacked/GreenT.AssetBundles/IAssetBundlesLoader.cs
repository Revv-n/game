using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace GreenT.AssetBundles;

public interface IAssetBundlesLoader
{
	IObservable<Unit> PreloadAsset(string path);

	IObservable<IAssetBundle> DownloadAssetBundle(string path);

	IObservable<T> GetAsset<T>(string path, string fileName) where T : UnityEngine.Object;

	IObservable<IEnumerable<T>> GetAssets<T>(string path) where T : UnityEngine.Object;

	IObservable<Unit> Init();

	void Release(string bundleName);
}
