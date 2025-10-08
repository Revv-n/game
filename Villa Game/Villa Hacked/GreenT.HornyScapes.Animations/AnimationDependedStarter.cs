using System;
using UniRx;

namespace GreenT.HornyScapes.Animations;

public class AnimationDependedStarter : AnimationStarter
{
	private IDisposable animationStream;

	public override void PlayAnimation()
	{
		animationStream?.Dispose();
		if ((bool)startPoint)
		{
			startPoint.Play();
		}
		base.PlayAnimation();
		animationStream = ObservableExtensions.Subscribe<AnimationController>(Observable.First<AnimationController>(controller.OnPlayEnd), (Action<AnimationController>)delegate
		{
			onEnd.OnNext(this);
		}, (Action<Exception>)delegate(Exception ex)
		{
			ex.LogException();
		});
	}

	public override void ResetToAnimStart()
	{
		ResetAnimation();
	}
}
