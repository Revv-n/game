using DG.Tweening;
using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.UI;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Messenger.UI;

public class LayoutAnimatedDisplayStrategy : MonoDisplayStrategy
{
	[SerializeField]
	private GreenT.HornyScapes.Animations.Animation shiftLayoutAnimation;

	[SerializeField]
	private GreenT.HornyScapes.Animations.Animation openAnimation;

	[SerializeField]
	private GreenT.HornyScapes.Animations.Animation closeAnimation;

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
			Observable.TimerFrame(1).Subscribe(delegate
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
