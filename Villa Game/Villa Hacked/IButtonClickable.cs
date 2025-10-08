using UnityEngine;

public class IButtonClickable : MonoBehaviour, IRayClickable
{
	bool IRayClickable.IsRayClickEnable => base.gameObject.activeSelf;

	RayClickOrder IRayClickable.RayClickOrder => new RayClickOrder(100, 100);

	void IRayClickable.AtRayClick()
	{
	}
}
