using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Animations;
using GreenT.UI;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.UI;

[RequireComponent(typeof(DefaultWindowAnimController))]
public class AnimatedWindow : Window
{
	[SerializeField]
	private DefaultWindowAnimController animationController;

	private CompositeDisposable closeAnimationStream = new CompositeDisposable();

	private bool isAnimating;

	public override void Init(IWindowsManager windowsOpener)
	{
		base.Init(windowsOpener);
		animationController.Init();
	}

	public override void Open()
	{
		base.Open();
		if (!isAnimating)
		{
			isAnimating = true;
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Animation>(Observable.DoOnCancel<Animation>(animationController.PlayShowAnimation(), (Action)delegate
			{
				isAnimating = false;
			}), (Action<Animation>)delegate
			{
				isAnimating = false;
			}), (ICollection<IDisposable>)closeAnimationStream);
		}
	}

	public override void Close()
	{
		if (!isAnimating)
		{
			isAnimating = true;
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Animation>(Observable.DoOnCancel<Animation>(animationController.PlayCloseAnimation(), (Action)delegate
			{
				Close();
			}), (Action<Animation>)delegate
			{
				Close();
			}), (ICollection<IDisposable>)closeAnimationStream);
		}
		void Close()
		{
			base.Close();
			isAnimating = false;
		}
	}

	protected virtual void OnDisable()
	{
		CompositeDisposable obj = closeAnimationStream;
		if (obj != null)
		{
			obj.Dispose();
		}
	}

	private new void OnValidate()
	{
		if (animationController == null)
		{
			animationController = GetComponent<DefaultWindowAnimController>();
		}
	}
}
