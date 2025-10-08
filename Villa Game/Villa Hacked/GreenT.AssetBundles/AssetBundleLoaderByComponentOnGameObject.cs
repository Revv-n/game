using System;
using GreenT.Settings;
using GreenT.Settings.Data;
using StripClub.Model.Data;
using UniRx;
using UnityEngine;

namespace GreenT.AssetBundles;

public class AssetBundleLoaderByComponentOnGameObject<K, T> : AssetBundleLoaderByType<K, GameObject>, IBundlesLoader<K, T>, ILoader<K, T> where T : Component
{
	public AssetBundleLoaderByComponentOnGameObject(IAssetBundlesLoader assetBundlesLoader, IProjectSettings projectSettings, BundleType bundleType)
		: base(assetBundlesLoader, projectSettings, bundleType)
	{
	}

	IObservable<T> ILoader<K, T>.Load(K param)
	{
		return Observable.Select<GameObject, T>(Load(param), (Func<GameObject, T>)((GameObject _gameObject) => _gameObject.GetComponent<T>()));
	}
}
