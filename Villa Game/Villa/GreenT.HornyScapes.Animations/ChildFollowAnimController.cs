using System;
using DG.Tweening;
using UnityEngine;

namespace GreenT.HornyScapes.Animations;

public class ChildFollowAnimController : AnimationController
{
	[SerializeField]
	private Transform child;

	[SerializeField]
	private Animation animation;

	private Vector3 childGlobalPosition;

	public Animation Animation => animation;

	public override void Init()
	{
		childGlobalPosition = child.transform.position;
		animation.Init();
	}

	public override void ResetToAnimStart()
	{
		ResetAnimation();
	}

	public override void PlayAllAnimations()
	{
		Play();
	}

	public override Sequence Play()
	{
		child.transform.position = childGlobalPosition;
		sequence = base.Play().Append(animation.Play()).OnStepComplete(Complete);
		return sequence;
	}

	public override void Stop()
	{
		animation.Stop();
	}

	public override void ResetAnimation()
	{
		child.transform.position = childGlobalPosition;
		Stop();
	}

	public override bool IsPlaying()
	{
		throw new NotImplementedException();
	}
}
