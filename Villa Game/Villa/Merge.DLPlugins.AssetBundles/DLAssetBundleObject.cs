using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Merge.DLPlugins.AssetBundles;

[Serializable]
public struct DLAssetBundleObject
{
	[SerializeField]
	[JsonProperty]
	private string name;

	[SerializeField]
	[JsonProperty]
	private string file;

	[SerializeField]
	[JsonProperty]
	private string manifest;

	[SerializeField]
	[JsonProperty("AssetFileHash")]
	private string hash;

	[JsonIgnore]
	public string Name => name;

	[JsonIgnore]
	public string Url => file;

	[JsonIgnore]
	public Hash128 Hash => Hash128.Parse(hash);

	public static implicit operator bool(DLAssetBundleObject obj)
	{
		return !string.IsNullOrEmpty(obj.name);
	}
}
