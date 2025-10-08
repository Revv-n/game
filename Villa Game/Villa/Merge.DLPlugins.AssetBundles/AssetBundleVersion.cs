using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Merge.DLPlugins.AssetBundles;

[Serializable]
public struct AssetBundleVersion
{
	[SerializeField]
	[JsonProperty]
	private string m_Hash;

	[SerializeField]
	[JsonProperty]
	private int m_Version;

	[JsonIgnore]
	public Hash128 Hash => Hash128.Parse(m_Hash);

	[JsonIgnore]
	public int Version => m_Version;

	[JsonConstructor]
	public AssetBundleVersion(Hash128 hash, int version)
	{
		m_Hash = hash.ToString();
		m_Version = version;
	}

	public override string ToString()
	{
		return $"{this}; Version: {Version}; Hash: {Hash}";
	}
}
