using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace GreenT.AssetBundles;

public class AssetLoadStrategy<TAsset> : AssetLoadStrategy where TAsset : Object
{
	public override AsyncOperationHandle LoadAsset(IResourceLocation location)
	{
		return Addressables.LoadAssetAsync<TAsset>(location);
	}
}
public abstract class AssetLoadStrategy
{
	public abstract AsyncOperationHandle LoadAsset(IResourceLocation location);
}
