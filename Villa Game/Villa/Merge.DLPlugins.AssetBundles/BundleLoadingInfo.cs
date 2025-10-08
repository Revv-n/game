using UnityEngine;

namespace Merge.DLPlugins.AssetBundles;

public class BundleLoadingInfo
{
	private BundleCompletionCallback m_OnComplete;

	public string Url { get; protected set; }

	public BundleLoadingType LoadingType { get; private set; }

	public string Name { get; protected set; }

	public long Size { get; protected set; }

	public bool Async { get; protected set; }

	public string NormalizedUnit
	{
		get
		{
			string result = "MB";
			if (NormalizedSize < 0.5)
			{
				result = "KB";
			}
			return result;
		}
	}

	public double NormalizedSize
	{
		get
		{
			double num = (double)Size / 1000000.0;
			if (num < 0.5)
			{
				num = (double)Size / 1000.0;
			}
			return num;
		}
	}

	public void SetSize(long value)
	{
		Size = value;
	}

	public void InvokeOnComplete(AssetBundle assetBundle, bool result)
	{
		m_OnComplete?.Invoke(assetBundle, result);
	}

	public BundleLoadingInfo(string fullUrl, string bundleName, long bundleSize, bool async, BundleLoadingType loadingType, BundleCompletionCallback onComplete = null)
	{
		Url = fullUrl;
		Name = bundleName;
		Size = bundleSize;
		Async = async;
		LoadingType = loadingType;
		m_OnComplete = onComplete;
	}
}
