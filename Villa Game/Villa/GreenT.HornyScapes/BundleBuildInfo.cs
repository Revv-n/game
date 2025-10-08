using System;
using System.Collections.Generic;
using UnityEngine;

namespace GreenT.HornyScapes;

[Serializable]
public struct BundleBuildInfo : IEquatable<BundleBuildInfo>
{
	public string name;

	public Hash128 bundleHash;

	public uint crc;

	public string contentHash;

	public DateTime timeStamp;

	public List<AssetDateInfo> assetsInfoDate;

	public BundleBuildInfo(string name, Hash128 bundleHash, uint crc, string contentHash, DateTime timeStamp, List<AssetDateInfo> assetsInfoDate)
	{
		this.name = name;
		this.bundleHash = bundleHash;
		this.crc = crc;
		this.contentHash = contentHash;
		this.timeStamp = timeStamp;
		this.assetsInfoDate = assetsInfoDate;
	}

	public override string ToString()
	{
		return string.Format("Name: {0} Unity hash: {1} Content MD5: {2} CRC: {3}", name, bundleHash, contentHash, crc.ToString("X8"));
	}

	public bool Equals(BundleBuildInfo other)
	{
		if (name == other.name && bundleHash.Equals(other.bundleHash) && crc == other.crc && contentHash == other.contentHash)
		{
			return timeStamp.Equals(other.timeStamp);
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj is BundleBuildInfo other)
		{
			return Equals(other);
		}
		return false;
	}

	public bool EqualBundleHash(BundleBuildInfo info)
	{
		return bundleHash == info.bundleHash;
	}

	public bool EqualTimeStamp(BundleBuildInfo info)
	{
		return timeStamp == info.timeStamp;
	}

	public bool NotEqualContentHash(BundleBuildInfo info)
	{
		return contentHash != info.contentHash;
	}

	public bool NotEqualCRC(BundleBuildInfo info)
	{
		return crc != info.crc;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(name, bundleHash, crc, contentHash, timeStamp);
	}
}
