using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Merge.ResourcesManagement;

public class ResourcesLoaderByFolder
{
	private readonly MonoBehaviour m_Host;

	public ResourcesLoaderByFolder(MonoBehaviour hostToAsync)
	{
		m_Host = hostToAsync;
	}

	public bool TryLoadAsset<T>(string path, out T asset) where T : UnityEngine.Object
	{
		return (asset = LoadAsset<T>(path)) != null;
	}

	public T LoadAsset<T>(string path) where T : UnityEngine.Object
	{
		if (!ValidateName(path))
		{
			return null;
		}
		T val = Resources.Load<T>(path);
		if (val == null)
		{
			Debug.LogError("Can't load resource: " + path);
		}
		return val;
	}

	public Coroutine LoadAssetAsync<T>(string path, Action<T, bool> callback) where T : UnityEngine.Object
	{
		return m_Host?.StartCoroutine(path, callback);
	}

	private IEnumerator LoadingAssetAsync<T>(string path, Action<T, bool> callback) where T : UnityEngine.Object
	{
		if (!ValidateName(path))
		{
			callback?.Invoke(null, arg2: false);
			yield break;
		}
		ResourceRequest request = Resources.LoadAsync<T>(path);
		yield return request;
		T val = request.asset as T;
		if (val == null)
		{
			Debug.LogError("Can't load resource: " + path);
		}
		callback?.Invoke(val, val);
	}

	public bool TryLoadSubAssets<T>(string path, out T[] assets) where T : UnityEngine.Object
	{
		return TryLoadAllAssets<T>(path, out assets);
	}

	public T[] LoadSubAssets<T>(string path) where T : UnityEngine.Object
	{
		return LoadAllAssets<T>(path);
	}

	public bool TryLoadAllAssets<T>(string path, out T[] assets) where T : UnityEngine.Object
	{
		return (assets = LoadAllAssets<T>(path)).Any();
	}

	public T[] LoadAllAssets<T>(string path) where T : UnityEngine.Object
	{
		if (string.IsNullOrEmpty(path))
		{
			path = string.Empty;
		}
		T[] array = Resources.LoadAll<T>(path);
		if (array == null)
		{
			Debug.LogError("Can't load multiple resources: " + path);
			array = new T[0];
		}
		return array;
	}

	private bool ValidateName(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			return false;
		}
		return true;
	}
}
