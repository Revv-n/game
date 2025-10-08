using System;
using DG.Tweening;

public class TweenTimer : TimerBase
{
	private Tween tween;

	public TweenTimer(RefTimer timer, Action callback, Action<TimerStatus> tickCallback = null)
		: base(timer, callback, tickCallback)
	{
	}

	protected override void StartTimer()
	{
		tween = DOTween.To(() => base.Timer.Passed, base.OnTimerTick, base.Timer.TotalTime, base.Timer.TimeLeft).OnComplete(base.AtComplete).SetEase(Ease.Linear);
	}

	protected override void StopTimer()
	{
		tween?.Kill();
	}
}
