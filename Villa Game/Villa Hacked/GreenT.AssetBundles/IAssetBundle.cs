using System;
using UnityEngine;

namespace GreenT.AssetBundles;

public interface IAssetBundle
{
	string name { get; }

	UnityEngine.Object LoadAsset(string fileName, Type type);

	T LoadAsset<T>(string fileName) where T : UnityEngine.Object;

	T[] LoadAllAssets<T>() where T : UnityEngine.Object;

	UnityEngine.Object[] LoadAllAssets(Type type);

	string[] GetAllAssetNames();

	bool Contains(string name);

	void Unload(bool b);
}
