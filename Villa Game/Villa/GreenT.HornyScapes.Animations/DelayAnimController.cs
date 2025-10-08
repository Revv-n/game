using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GreenT.HornyScapes.Animations;

public class DelayAnimController : AnimationController
{
	[SerializeField]
	private AnimationDelayDictionary animations;

	public override void Init()
	{
		base.Init();
		foreach (KeyValuePair<Animation, float> animation in animations)
		{
			animation.Key.Init();
		}
	}

	public override void ResetToAnimStart()
	{
		ResetAnimation();
	}

	public override bool IsPlaying()
	{
		throw new NotImplementedException();
	}

	public override void PlayAllAnimations()
	{
		foreach (KeyValuePair<Animation, float> animation in animations)
		{
			AnimationController.TryPlayAnimation(animation.Key);
		}
		if (animations != null)
		{
			_ = animations.Count;
			_ = 1;
		}
	}

	public override void ResetAnimation()
	{
		KeyValuePair<Animation, float> keyValuePair = animations.FirstOrDefault();
		if ((bool)keyValuePair.Key)
		{
			keyValuePair.Key.ResetToAnimStart();
		}
	}

	protected override void OnValidate()
	{
		base.OnValidate();
		if (animations == null)
		{
			return;
		}
		foreach (KeyValuePair<Animation, float> animation in animations)
		{
			animation.Key.Delay = animation.Value;
		}
	}
}
