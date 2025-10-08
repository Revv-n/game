using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace GreenT.AssetBundles;

public class AddressableBundle : IAssetBundle
{
	private readonly IEnumerable<UnityEngine.Object> _assets;

	private readonly AsyncOperationHandle _operationHandle;

	private readonly string _name;

	public string name => _name;

	public AddressableBundle(IEnumerable<UnityEngine.Object> assets, AsyncOperationHandle operationHandle, string name)
	{
		_assets = assets;
		_operationHandle = operationHandle;
		_name = name;
	}

	public UnityEngine.Object LoadAsset(string fileName, Type type)
	{
		return _assets.Where((UnityEngine.Object x) => x.GetType() == type).First((UnityEngine.Object x) => x.name == fileName);
	}

	public T LoadAsset<T>(string fileName) where T : UnityEngine.Object
	{
		return _assets.OfType<T>().First((T x) => x.name == fileName);
	}

	public T[] LoadAllAssets<T>() where T : UnityEngine.Object
	{
		return _assets.OfType<T>().ToArray();
	}

	public UnityEngine.Object[] LoadAllAssets(Type type)
	{
		return _assets.Where((UnityEngine.Object x) => x.GetType() == type).ToArray();
	}

	public string[] GetAllAssetNames()
	{
		return _assets.Select((UnityEngine.Object x) => x.name).ToArray();
	}

	public bool Contains(string name)
	{
		return _assets.Any((UnityEngine.Object x) => x.name == name);
	}

	public void Unload(bool b)
	{
		Addressables.Release(_operationHandle);
	}
}
