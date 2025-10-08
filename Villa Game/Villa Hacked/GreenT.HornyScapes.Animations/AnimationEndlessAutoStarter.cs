using System;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Animations;

public class AnimationEndlessAutoStarter : AnimationAutoStarter
{
	private IDisposable endlessStream;

	protected override void OnEnable()
	{
		base.OnEnable();
		endlessStream?.Dispose();
		ObservableExtensions.Subscribe<AnimationController>(Observable.DoOnCancel<AnimationController>(Observable.TakeUntilDisable<AnimationController>(controller.OnPlayEnd, (Component)this), (Action)ResetAnimation), (Action<AnimationController>)delegate
		{
			base.OnEnable();
		});
	}

	protected override void OnDisable()
	{
		endlessStream?.Dispose();
		base.OnDisable();
	}
}
