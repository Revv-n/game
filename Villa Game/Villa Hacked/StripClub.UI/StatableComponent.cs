using UnityEngine;

namespace StripClub.UI;

public abstract class StatableComponent : MonoBehaviour, IStatable
{
	public abstract void Set(int stateNumber);
}
