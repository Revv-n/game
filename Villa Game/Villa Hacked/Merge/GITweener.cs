using System;
using DG.Tweening;

namespace Merge;

public class GITweener
{
	public Tween Tween { get; private set; }

	public GITweener(Tween tween)
	{
		Tween = tween;
	}

	public void AddOnCompleteAction(Action callback)
	{
		Tween tween = Tween;
		tween.onComplete = (TweenCallback)Delegate.Combine(tween.onComplete, (TweenCallback)delegate
		{
			callback();
		});
	}

	public void Kill()
	{
		Tween?.Kill();
	}
}
