using System;
using UnityEngine;

namespace GreenT.HornyScapes.Animations;

public class LayoutGroupComponentAnimController : AnimationController
{
	[SerializeField]
	private Animation controller;

	[SerializeField]
	private Animation[] animationsWithDelay;

	protected override void OnValidate()
	{
		if (!controller)
		{
			Debug.LogWarning("controller is empty", this);
		}
	}

	public override void Init()
	{
		if ((bool)controller)
		{
			controller.Init();
		}
	}

	public override void ResetToAnimStart()
	{
		ResetAnimation();
	}

	public override void PlayAllAnimations()
	{
		if ((bool)controller)
		{
			controller.Play();
		}
	}

	public override void ResetAnimation()
	{
		if ((bool)controller)
		{
			controller.ResetToAnimStart();
		}
	}

	public void SetDelay(float delay)
	{
		Animation[] array = animationsWithDelay;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Delay = delay;
		}
	}

	public override bool IsPlaying()
	{
		throw new NotImplementedException();
	}
}
