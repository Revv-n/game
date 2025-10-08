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
		animationStream = controller.OnPlayEnd.First().Subscribe(delegate
		{
			onEnd.OnNext(this);
		}, delegate(Exception ex)
		{
			ex.LogException();
		});
	}

	public override void ResetToAnimStart()
	{
		ResetAnimation();
	}
}
