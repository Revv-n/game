using System.IO;

namespace Merge.DLPlugins.AssetBundles;

public class AssetBundleDelegates
{
	public BundleCompletionCallback bundleCompletionCallback;

	public AsyncLoadingBundleCallback asyncLoadingBundleCallback;

	public ManifestCompletionCallback manifestCompletionCallback;

	public BundleProgressCallback bundleProgressCallback;

	public BundleLoadingCompleted bundleLoadingCompleted;

	public BundleLoadingStarted bundleLoadingStarted;

	public ConnectionError connectionError;

	public MessegeLog messegeLog;

	public MessegeLogError messegeLogError;

	public string Combine(params string[] args)
	{
		return Path.Combine(args).Replace('\\', '/');
	}
}
