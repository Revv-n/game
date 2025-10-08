using System.Collections.Generic;
using UnityEngine;

namespace Merge.DLPlugins.AssetBundles;

[CreateAssetMenu(fileName = "AssetBundlesConfig", menuName = "DL/Configs/AssetBundles Config")]
public class AssetBundlesConfig : ScriptableObject
{
	[SerializeField]
	private List<string> streamingAssetBundles = new List<string> { "localization", "balance", "event0" };

	public List<string> StreamingAssetBundles => streamingAssetBundles;
}
