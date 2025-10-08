using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Animations;

public class AnimationContinuousStarter : AnimationStarter
{
	[SerializeField]
	private List<AnimationController> nextControllers = new List<AnimationController>();

	[SerializeField]
	private bool resetOnlyMain = true;

	private CompositeDisposable disposables = new CompositeDisposable();

	protected override void OnValidate()
	{
		base.OnValidate();
		if (nextControllers == null || nextControllers.Count == 0)
		{
			Debug.LogError("Continuous Starter can't have empty a controller list", this);
		}
	}

	public override void Init()
	{
		base.Init();
		foreach (AnimationController nextController in nextControllers)
		{
			nextController.Init();
		}
	}

	public override void ResetToAnimStart()
	{
		ResetAnimation();
	}

	public override void PlayAnimation()
	{
		CompositeDisposable obj = disposables;
		if (obj != null)
		{
			obj.Clear();
		}
		base.PlayAnimation();
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<AnimationController>(Observable.First<AnimationController>(controller.OnPlayEnd), (Action<AnimationController>)delegate
		{
			nextControllers[0].PlayAllAnimations();
		}, (Action<Exception>)delegate(Exception ex)
		{
			ex.LogException();
		}), (ICollection<IDisposable>)disposables);
		int ind;
		for (ind = 1; ind < nextControllers.Count - 1; ind++)
		{
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<AnimationController>(Observable.First<AnimationController>(nextControllers[ind].OnPlayEnd), (Action<AnimationController>)delegate
			{
				nextControllers[ind + 1].PlayAllAnimations();
			}, (Action<Exception>)delegate(Exception ex)
			{
				ex.LogException();
			}), (ICollection<IDisposable>)disposables);
		}
	}

	public override void ResetAnimation()
	{
		if (resetOnlyMain)
		{
			base.ResetAnimation();
			return;
		}
		foreach (AnimationController nextController in nextControllers)
		{
			nextController.ResetAnimation();
		}
	}
}
