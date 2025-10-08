using System.Collections.Generic;

namespace GreenT.AssetBundles;

public class AddressableBundleCache : IAssetBundlesCache
{
	public IDictionary<string, IAssetBundle> Cache { get; } = new Dictionary<string, IAssetBundle>();


	public void AddBundle(string bundleName, IAssetBundle assetBundle)
	{
		Cache.Add(bundleName, assetBundle);
	}

	public IAssetBundle GetBundle(string bundleName)
	{
		return Cache[bundleName];
	}

	public void Release(string bundleName)
	{
		Cache[bundleName].Unload(b: true);
		Cache.Remove(bundleName);
	}

	public void ReleaseAll()
	{
		foreach (IAssetBundle value in Cache.Values)
		{
			value.Unload(b: true);
		}
		Cache.Clear();
	}
}
