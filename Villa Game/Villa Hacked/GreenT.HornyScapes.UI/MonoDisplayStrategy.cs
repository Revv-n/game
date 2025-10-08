using UnityEngine;

namespace GreenT.HornyScapes.UI;

public class MonoDisplayStrategy : MonoBehaviour, IDisplayStrategy
{
	[SerializeField]
	protected GameObject targetObject;

	public virtual void Display(bool display)
	{
		targetObject.SetActive(display);
	}

	private void OnValidate()
	{
		if (targetObject == null)
		{
			targetObject = base.gameObject;
		}
	}
}
