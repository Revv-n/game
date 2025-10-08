using UnityEngine;

namespace GreenT.HornyScapes.UI;

public abstract class ObjectCollection<T, K> : MonoBehaviour
{
	public abstract K Get(T param);
}
