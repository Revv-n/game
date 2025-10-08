using System;
using UnityEngine;

namespace GreenT.HornyScapes.Animations;

public class AnimatedGridLayoutGroupController : AnimationController
{
	[SerializeField]
	private float delay;

	[SerializeField]
	private float delayBetweenElements = 0.5f;

	[SerializeField]
	private AnimatedGridLayoutGroup animatedLayoutGroup;

	public override void Init()
	{
		base.Init();
		animatedLayoutGroup.UpdateGroupChildren();
	}

	public override void ResetToAnimStart()
	{
		ResetAnimation();
	}

	public override void PlayAllAnimations()
	{
	}

	public override void ResetAnimation()
	{
		animatedLayoutGroup.ResetAnimations();
	}

	public override bool IsPlaying()
	{
		throw new NotImplementedException();
	}
}
