using System;
using GreenT.HornyScapes.Animations;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.UI;

public class MonoViewAnimatedDisplayStrategy : MonoDisplayStrategy, IDisplayStrategy
{
	[SerializeField]
	private AnimationController openAnimation;

	[SerializeField]
	private AnimationController closeAnimation;

	private IDisposable disposable;

	public void Awake()
	{
		openAnimation.Init();
		closeAnimation.Init();
	}

	public override void Display(bool display)
	{
		if (display)
		{
			Show();
		}
		else
		{
			Hide();
		}
	}

	public void Show()
	{
		closeAnimation.ResetAnimation();
		base.Display(display: true);
		openAnimation.PlayAllAnimations();
	}

	public void Hide()
	{
		if (openAnimation.IsPlaying())
		{
			disposable?.Dispose();
			disposable = ObservableExtensions.Subscribe<IObservable<AnimationController>>(Observable.FirstOrDefault<IObservable<AnimationController>>(Observable.Select<AnimationController, IObservable<AnimationController>>(Observable.Do<AnimationController>(Observable.FirstOrDefault<AnimationController>(openAnimation.OnPlayEnd), (Action<AnimationController>)delegate
			{
				closeAnimation.PlayAllAnimations();
			}), (Func<AnimationController, IObservable<AnimationController>>)((AnimationController _) => closeAnimation.OnPlayEnd))), (Action<IObservable<AnimationController>>)delegate
			{
				base.Display(display: false);
			});
		}
		else
		{
			if (closeAnimation.IsPlaying())
			{
				return;
			}
			disposable?.Dispose();
			if (targetObject.activeSelf && targetObject.activeInHierarchy)
			{
				closeAnimation.PlayAllAnimations();
				disposable = ObservableExtensions.Subscribe<AnimationController>(Observable.FirstOrDefault<AnimationController>(closeAnimation.OnPlayEnd), (Action<AnimationController>)delegate
				{
					base.Display(display: false);
				});
			}
			else
			{
				base.Display(display: false);
			}
		}
	}

	protected virtual void OnDisable()
	{
		disposable?.Dispose();
		openAnimation.ResetAnimation();
		closeAnimation.ResetAnimation();
	}
}
