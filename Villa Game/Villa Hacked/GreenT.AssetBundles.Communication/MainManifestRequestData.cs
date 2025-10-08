namespace GreenT.AssetBundles.Communication;

public struct MainManifestRequestData
{
	public readonly string name;

	public readonly uint retryCount;

	private readonly string _configVersion;

	public readonly string url;

	public readonly string fileUrl => url + "/" + name + ".customManifest";

	public MainManifestRequestData(string name, string url, uint retryCount, string configVersion)
	{
		this.name = name;
		this.retryCount = retryCount;
		_configVersion = configVersion;
		this.url = url;
	}
}
