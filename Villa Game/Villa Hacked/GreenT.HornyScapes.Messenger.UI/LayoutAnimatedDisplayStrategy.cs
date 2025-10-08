using System;
using DG.Tweening;
using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.UI;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Messenger.UI;

public class LayoutAnimatedDisplayStrategy : MonoDisplayStrategy
{
	[SerializeField]
	private Animation shiftLayoutAnimation;

	[SerializeField]
	private Animation openAnimation;

	[SerializeField]
	private Animation closeAnimation;

	public bool debug;

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
			ObservableExtensions.Subscribe<long>(Observable.TimerFrame(1, (FrameCountType)0), (Action<long>)delegate
			{
				shiftLayoutAnimation.Stop();
				openAnimation.Play();
				if (debug)
				{
					Debug.Log("Stopped and play");
				}
			});
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
