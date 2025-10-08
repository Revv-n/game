using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace GreenT.AssetBundles;

public class AddressableAssetCache : IAssetCache
{
	private IDictionary<string, AsyncOperationHandle> _cache { get; } = new Dictionary<string, AsyncOperationHandle>();


	private IDictionary<string, AsyncOperationHandle> _preloadCache { get; } = new Dictionary<string, AsyncOperationHandle>();


	public void Add(string assetName, AsyncOperationHandle handle)
	{
		_cache.Add(assetName, handle);
	}

	public void AddPreload(string assetName, AsyncOperationHandle handle)
	{
		_preloadCache.Add(assetName, handle);
	}

	public T GetAsset<T>(string assetName) where T : Object
	{
		return _cache[assetName].Result as T;
	}

	public bool HasAsset(string assetName)
	{
		return _cache.ContainsKey(assetName);
	}

	public bool HasPreloadAsset(string assetName)
	{
		return _preloadCache.ContainsKey(assetName);
	}

	public void Release(string assetName)
	{
		AsyncOperationHandle handle = _cache[assetName];
		_cache.Remove(assetName);
		Addressables.Release(handle);
	}

	public void ReleaseAll()
	{
		foreach (AsyncOperationHandle value in _cache.Values)
		{
			Addressables.Release(value);
		}
		_cache.Clear();
	}
}
