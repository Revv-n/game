using System;
using UnityEngine;

namespace GreenT.AssetBundles;

public class AssetBundleWrapper : IAssetBundle
{
	public readonly AssetBundle AssetBundle;

	public string name => ((UnityEngine.Object)(object)AssetBundle).name;

	public AssetBundleWrapper(AssetBundle assetBundle)
	{
		AssetBundle = assetBundle;
	}

	public UnityEngine.Object LoadAsset(string fileName, Type type)
	{
		return AssetBundle.LoadAsset(fileName, type);
	}

	public T LoadAsset<T>(string fileName) where T : UnityEngine.Object
	{
		return AssetBundle.LoadAsset<T>(fileName);
	}

	public T[] LoadAllAssets<T>() where T : UnityEngine.Object
	{
		return AssetBundle.LoadAllAssets<T>();
	}

	public UnityEngine.Object[] LoadAllAssets(Type type)
	{
		return AssetBundle.LoadAllAssets(type);
	}

	public string[] GetAllAssetNames()
	{
		return AssetBundle.GetAllAssetNames();
	}

	public bool Contains(string name)
	{
		return AssetBundle.Contains(name);
	}

	public void Unload(bool unloadAllLoadedObjects)
	{
		AssetBundle.Unload(unloadAllLoadedObjects);
	}
}
