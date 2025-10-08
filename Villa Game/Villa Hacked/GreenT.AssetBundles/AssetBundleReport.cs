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
		return $"Name Bundle: {name}\nUrl Bundle: {url}\nHash: {hash}\nIs Cached: {isCached}\nTime Download: {timeDownload.TotalSeconds} sec.";
	}
}
