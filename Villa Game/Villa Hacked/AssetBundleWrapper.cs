using System;
using UnityEngine;

public class AssetBundleWrapper
{
	private readonly AssetBundle _assetBundle;

	public AssetBundleWrapper(AssetBundle assetBundle)
	{
		_assetBundle = assetBundle;
	}

	public T LoadAsset<T>(string name) where T : UnityEngine.Object
	{
		return _assetBundle.LoadAsset<T>(name);
	}

	public T[] LoadAssets<T>() where T : UnityEngine.Object
	{
		return _assetBundle.LoadAllAssets<T>();
	}

	public void LoadAssetAsync<T>(string name, Action<T> result) where T : UnityEngine.Object
	{
		_assetBundle.LoadAssetAsync<T>(name);
	}

	public void LoadAssetsAsync<T>(Action<T[]> result) where T : UnityEngine.Object
	{
		_assetBundle.LoadAllAssetsAsync<T>();
	}

	public string[] GetAllScenePaths()
	{
		return _assetBundle.GetAllScenePaths();
	}

	public void Unload(bool includeAllLoadedAssets = false)
	{
		_assetBundle.Unload(includeAllLoadedAssets);
	}
}
