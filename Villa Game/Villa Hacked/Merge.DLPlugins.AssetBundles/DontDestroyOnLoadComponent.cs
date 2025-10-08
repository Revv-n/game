using UnityEngine;

namespace Merge.DLPlugins.AssetBundles;

internal class DontDestroyOnLoadComponent : MonoBehaviour
{
	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
	}
}
