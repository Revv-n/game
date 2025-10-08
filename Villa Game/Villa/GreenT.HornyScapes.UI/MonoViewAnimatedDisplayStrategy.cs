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
			disposable = (from _ in openAnimation.OnPlayEnd.FirstOrDefault().Do(delegate
				{
					closeAnimation.PlayAllAnimations();
				})
				select closeAnimation.OnPlayEnd).FirstOrDefault().Subscribe(delegate
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
				disposable = closeAnimation.OnPlayEnd.FirstOrDefault().Subscribe(delegate
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
