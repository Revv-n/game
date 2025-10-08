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
		disposables?.Clear();
		base.PlayAnimation();
		controller.OnPlayEnd.First().Subscribe(delegate
		{
			nextControllers[0].PlayAllAnimations();
		}, delegate(Exception ex)
		{
			ex.LogException();
		}).AddTo(disposables);
		int ind;
		for (ind = 1; ind < nextControllers.Count - 1; ind++)
		{
			nextControllers[ind].OnPlayEnd.First().Subscribe(delegate
			{
				nextControllers[ind + 1].PlayAllAnimations();
			}, delegate(Exception ex)
			{
				ex.LogException();
			}).AddTo(disposables);
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
