using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GreenT.AssetBundles.Scheduler;

public static class AssetBundleScheduler
{
	public static SerializableDictionary<string, QualityType> BundlesQuality = new SerializableDictionary<string, QualityType>();

	public static readonly string[] BundleFilters = new string[0];

	private const string MediaContent = "content/media";

	private const string MapContent = "meta/background";

	private const string PreloadedMapContent = "meta/background_preload";

	public const string VariantSdURL = ".sd";

	public static bool HasBundleQuality(string url, QualityType quality)
	{
		if (BundlesQuality.ContainsKey(url))
		{
			return BundlesQuality[url] == quality;
		}
		return false;
	}

	public static bool GetNextURLBundle(string url, string bundleName, out string result)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		bool num = HasSdOrHd(url);
		result = new string(string.op_Implicit(url));
		if (num)
		{
			TryChangeSdToHd(url);
			return false;
		}
		bool flag = IsCachedHDOnDisk(bundleName);
		if (BundleFilters.Any(url.Contains) && !flag)
		{
			BundlesQuality.Add(url, QualityType.SD);
			result += ".sd";
		}
		else
		{
			BundlesQuality.Add(url, QualityType.HD);
		}
		if (url.Contains("content/media"))
		{
			int mediaID = MediaQuality.GetMediaID(url);
			MediaQuality.Info[mediaID] = BundlesQuality[url];
		}
		return true;
	}

	public static string AddSDIfUrlSD(string url, string bundleName)
	{
		if (url.Contains(".sd"))
		{
			bundleName = CreateSDVariant(bundleName);
		}
		return bundleName;
	}

	public static bool HasSDVariant(string bundleName)
	{
		return BundlesQuality.Keys.Any((string key) => key.Contains(bundleName));
	}

	public static string FindSDVariant(string bundleName)
	{
		if (HasSDVariant(bundleName))
		{
			return CreateSDVariant(bundleName);
		}
		return bundleName;
	}

	public static string CreateSDVariant(string bundleName)
	{
		bundleName += ".sd";
		return bundleName;
	}

	private static bool HasSdOrHd(string url)
	{
		return BundlesQuality.ContainsKey(url);
	}

	private static bool IsCachedHDOnDisk(string bundleName)
	{
		return new List<Hash128>().Any();
	}

	private static void TryChangeSdToHd(string url)
	{
		if (BundleFilters.Any(url.Contains) && BundlesQuality[url] == QualityType.SD)
		{
			BundlesQuality[url] = QualityType.HD;
		}
	}
}
