using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GreenT.HornyScapes.Animations;

public class DefaultWindowAnimController : AnimationController
{
	[SerializeField]
	private List<Animation> showAnimations;

	[SerializeField]
	private List<Animation> hideAnimations;

	private List<Animation> playList = new List<Animation>();

	public override void Init()
	{
		base.Init();
		foreach (Animation showAnimation in showAnimations)
		{
			showAnimation.Init();
		}
		foreach (Animation hideAnimation in hideAnimations)
		{
			hideAnimation.Init();
		}
		playList = showAnimations;
	}

	public override void ResetToAnimStart()
	{
		ResetAnimation();
	}

	public IObservable<Animation> PlayShowAnimation()
	{
		playList = showAnimations;
		PlayAllAnimations();
		return showAnimations.Last().OnAnimationEnd;
	}

	public IObservable<Animation> PlayCloseAnimation()
	{
		playList = hideAnimations;
		PlayAllAnimations();
		return hideAnimations.Last().OnAnimationEnd;
	}

	public override void PlayAllAnimations()
	{
		foreach (Animation play in playList)
		{
			AnimationController.TryPlayAnimation(play);
		}
	}

	public override void ResetAnimation()
	{
		Animation animation = showAnimations.FirstOrDefault();
		if ((bool)animation)
		{
			animation.ResetToAnimStart();
		}
	}

	public override bool IsPlaying()
	{
		throw new NotImplementedException();
	}
}
