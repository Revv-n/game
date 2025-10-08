using GreenT.HornyScapes;

namespace GreenT.AssetBundles.Communication;

public struct AssetBundleRequestData
{
	public readonly string name;

	public readonly string url;

	public readonly uint retryCount;

	public readonly BuildMainInfo BuildMainInfo;

	public BundleBuildInfo? previousBundleInfo;

	public string bundleUrl => AddHashToUrl(url ?? "");

	public string manifestUrl => AddHashToUrl(url + ".manifest");

	public string customManifestUrl => AddHashToUrl(url + ".customManifest");

	private string AddHashToUrl(string url)
	{
		return url;
	}

	public AssetBundleRequestData(string name, string url, uint retryCount, BuildMainInfo buildMainInfo, BundleBuildInfo? previousBundleInfo = null)
	{
		this.name = name;
		this.url = url;
		this.retryCount = retryCount;
		BuildMainInfo = buildMainInfo;
		this.previousBundleInfo = previousBundleInfo;
	}
}
