using DG.Tweening;
using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.UI;
using UnityEngine;

namespace GreenT.HornyScapes.Messenger.UI;

public class AnimatedDisplayStrategy : MonoDisplayStrategy
{
	[SerializeField]
	private GreenT.HornyScapes.Animations.Animation openAnimation;

	[SerializeField]
	private GreenT.HornyScapes.Animations.Animation closeAnimation;

	private void Awake()
	{
		openAnimation.Init();
		closeAnimation.Init();
	}

	public override void Display(bool display)
	{
		if (display)
		{
			base.Display(display: true);
			openAnimation.Play();
		}
		else
		{
			closeAnimation.Play().OnComplete(OnCloseAnimationComplete);
		}
	}

	private void OnCloseAnimationComplete()
	{
		base.Display(display: false);
	}
}
