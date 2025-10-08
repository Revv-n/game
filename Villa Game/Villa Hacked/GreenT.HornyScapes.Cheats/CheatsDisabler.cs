using UnityEngine;

namespace GreenT.HornyScapes.Cheats;

public class CheatsDisabler : MonoBehaviour
{
	private void Awake()
	{
		Object.Destroy(base.gameObject);
	}
}
