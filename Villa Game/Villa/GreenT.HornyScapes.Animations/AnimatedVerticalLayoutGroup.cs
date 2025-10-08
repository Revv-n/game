using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.Animations;

public class AnimatedVerticalLayoutGroup : VerticalLayoutGroup
{
	[SerializeField]
	private float delay;

	[SerializeField]
	private float delayBetweenElements = 0.5f;

	[Tooltip("Initialize animation everytime when layout rebuild")]
	[SerializeField]
	private bool initialize = true;

	[Tooltip("Reset animation everytime when layout rebuild")]
	[SerializeField]
	private bool reset = true;

	[SerializeField]
	private AnimationGroupID affectedGroupID;

	private Animation[] animations;

	public void ResetAnimations()
	{
		Animation[] array = animations;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].ResetToAnimStart();
		}
	}

	public void InitializeAnimations()
	{
		Animation[] array = animations;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Init();
		}
	}

	public void UpdateGroupChildren()
	{
		animations = (from _animation in base.transform.GetComponentsInChildren<Animation>()
			where _animation.GroupID == affectedGroupID
			select _animation).ToArray();
		for (int i = 0; i < animations.Length; i++)
		{
			animations[i].Delay = delay + delayBetweenElements * (float)i;
		}
	}

	public override void SetLayoutVertical()
	{
		UpdateGroupChildren();
		if (initialize)
		{
			InitializeAnimations();
		}
		if (reset)
		{
			ResetAnimations();
		}
		base.SetLayoutVertical();
		Animation[] array = animations;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Play();
		}
	}
}
