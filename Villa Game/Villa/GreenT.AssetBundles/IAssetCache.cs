using UnityEngine;

namespace GreenT.AssetBundles;

public interface IAssetCache
{
	T GetAsset<T>(string assetPath) where T : Object;

	bool HasAsset(string assetPath);

	void Release(string assetPath);

	void ReleaseAll();
}
