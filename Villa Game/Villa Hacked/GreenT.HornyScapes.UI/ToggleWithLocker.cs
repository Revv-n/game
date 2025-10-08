using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.UI;

public class ToggleWithLocker : Toggle
{
	[SerializeField]
	private Image locker;

	public bool IsLock { get; private set; }

	public virtual void SetLocker(bool isLock)
	{
		IsLock = isLock;
		locker.gameObject.SetActive(IsLock);
		base.interactable = !IsLock;
		if (isLock && base.isOn)
		{
			base.isOn = false;
		}
	}
}
