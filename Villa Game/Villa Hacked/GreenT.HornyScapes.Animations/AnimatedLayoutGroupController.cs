using System;
using UnityEngine;

namespace GreenT.HornyScapes.Animations;

public class AnimatedLayoutGroupController : AnimationController
{
	[SerializeField]
	private float delay;

	[SerializeField]
	private float delayBetweenElements = 0.5f;

	[SerializeField]
	private AnimatedVerticalLayoutGroup animatedVerticalLayoutGroup;

	public void RecalculateAnimations()
	{
		animatedVerticalLayoutGroup.UpdateGroupChildren();
	}

	public override void PlayAllAnimations()
	{
	}

	public override void ResetAnimation()
	{
		animatedVerticalLayoutGroup.ResetAnimations();
	}

	public override bool IsPlaying()
	{
		throw new NotImplementedException();
	}

	public override void ResetToAnimStart()
	{
		ResetAnimation();
	}
}
