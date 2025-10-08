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
		return from _gameObject in Load(param)
			select _gameObject.GetComponent<T>();
	}
}
