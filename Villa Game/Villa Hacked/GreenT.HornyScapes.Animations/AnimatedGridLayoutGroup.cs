using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.Animations;

public class AnimatedGridLayoutGroup : GridLayoutGroup
{
	[SerializeField]
	private float delay;

	[SerializeField]
	private float delayBetweenElements = 0.5f;

	private LayoutGroupComponentAnimController[] childs;

	public void ResetAnimations()
	{
		LayoutGroupComponentAnimController[] array = childs;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].ResetAnimation();
		}
	}

	public void UpdateGroupChildren()
	{
		childs = base.transform.GetComponentsInChildren<LayoutGroupComponentAnimController>();
		for (int i = 0; i < childs.Length; i++)
		{
			childs[i].SetDelay(delay + delayBetweenElements * (float)i);
		}
	}

	public override void SetLayoutHorizontal()
	{
		UpdateGroupChildren();
		LayoutGroupComponentAnimController[] array = childs;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Init();
		}
		base.SetLayoutHorizontal();
		array = childs;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].PlayAllAnimations();
		}
	}
}
