using System;
using UnityEngine;

namespace GreenT.AssetBundles;

public struct AssetBundleReport
{
	public readonly string name;

	public readonly string url;

	public readonly bool isCached;

	public readonly Hash128 hash;

	public readonly TimeSpan timeDownload;

	public AssetBundleReport(string name, string url, bool isCached, Hash128 hash, TimeSpan timeDownload)
	{
		this.name = name;
		this.isCached = isCached;
		this.hash = hash;
		this.timeDownload = timeDownload;
		this.url = url;
	}

	public override string ToString()
	{
		object[] obj = new object[5] { name, url, hash, isCached, null };
		TimeSpan timeSpan = timeDownload;
		obj[4] = timeSpan.TotalSeconds;
		return string.Format("Name Bundle: {0}\nUrl Bundle: {1}\nHash: {2}\nIs Cached: {3}\nTime Download: {4} sec.", obj);
	}
}
