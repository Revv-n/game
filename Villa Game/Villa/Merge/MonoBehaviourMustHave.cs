using UnityEngine;

namespace Merge;

public static class MonoBehaviourMustHave
{
	public static void DestroyGameObject(this MonoBehaviour mb)
	{
		Object.Destroy(mb.gameObject);
	}

	public static void SetActive(this MonoBehaviour mb, bool active)
	{
		mb.gameObject?.SetActive(active);
	}
}
