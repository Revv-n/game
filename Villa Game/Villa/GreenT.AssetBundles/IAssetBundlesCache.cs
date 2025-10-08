using System.Collections.Generic;

namespace GreenT.AssetBundles;

public interface IAssetBundlesCache
{
	IDictionary<string, IAssetBundle> Cache { get; }

	IAssetBundle GetBundle(string bundleName);

	void Release(string bundleName);

	void ReleaseAll();
}
