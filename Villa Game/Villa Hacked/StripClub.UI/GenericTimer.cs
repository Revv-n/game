using System;
using UniRx;
using UnityEngine;

namespace StripClub.UI;

public class GenericTimer : IDisposable
{
	private static readonly TimeSpan oneSecond;

	private static readonly IObservable<bool> observablePause;

	private static float pauseTime;

	protected ReactiveProperty<bool> isActive = new ReactiveProperty<bool>(false);

	protected Subject<GenericTimer> timeIsUp = new Subject<GenericTimer>();

	protected Subject<TimeSpan> update = new Subject<TimeSpan>();

	private IDisposable timerStream;

	private IDisposable applicationPauseStream;

	private static IDisposable observePauseStream;

	public static TimeSpan PastPauseDuration { get; private set; }

	public TimeSpan InitTime { get; set; }

	public TimeSpan TimeLeft { get; set; }

	public IReadOnlyReactiveProperty<bool> IsActive => (IReadOnlyReactiveProperty<bool>)(object)isActive;

	public IObservable<GenericTimer> OnTimeIsUp => Observable.AsObservable<GenericTimer>((IObservable<GenericTimer>)timeIsUp);

	public IObservable<TimeSpan> OnUpdate => Observable.StartWith<TimeSpan>(Observable.AsObservable<TimeSpan>((IObservable<TimeSpan>)update), TimeLeft);

	public TimeSpan UpdatePeriod { get; set; } = oneSecond;


	static GenericTimer()
	{
		oneSecond = TimeSpan.FromSeconds(1.0);
		observePauseStream = null;
		PastPauseDuration = TimeSpan.Zero;
		observablePause = MainThreadDispatcher.OnApplicationPauseAsObservable();
		observePauseStream = ObservableExtensions.Subscribe<bool>(observablePause, (Action<bool>)OnApplicationPause);
	}

	public GenericTimer()
	{
		applicationPauseStream = ObservableExtensions.Subscribe<bool>(Observable.Where<bool>(observablePause, (Func<bool, bool>)((bool x) => !x && isActive.Value)), (Action<bool>)delegate
		{
			TimeLeft -= PastPauseDuration;
		});
	}

	public GenericTimer(TimeSpan timeLeft)
		: this()
	{
		Start(timeLeft);
	}

	public void Start(TimeSpan timeLeft)
	{
		TimeSpan timeLeft2 = (InitTime = timeLeft);
		TimeLeft = timeLeft2;
		StartTimer();
	}

	public void Resume()
	{
		StartTimer();
	}

	private void StartTimer()
	{
		timerStream?.Dispose();
		if (TimeLeft.Ticks < 0)
		{
			OnTimerComplete();
			return;
		}
		isActive.Value = true;
		timerStream = ObservableExtensions.Subscribe<long>(Observable.TakeWhile<long>(Observable.Timer(TimeSpan.Zero, UpdatePeriod, Scheduler.MainThreadIgnoreTimeScale), (Func<long, bool>)((long _) => TimeLeft.Ticks > 0)), (Action<long>)delegate
		{
			ActionOnEveryUpdate();
		}, (Action)OnTimerComplete);
	}

	public void Rewind(TimeSpan shift)
	{
		if (isActive.Value)
		{
			if (shift < TimeLeft)
			{
				TimeLeft -= shift;
			}
			else
			{
				TimeLeft = TimeSpan.Zero;
			}
		}
	}

	private static void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			pauseTime = Time.realtimeSinceStartup;
			return;
		}
		PastPauseDuration = TimeSpan.FromSeconds(Time.realtimeSinceStartup - pauseTime);
		pauseTime = Time.realtimeSinceStartup;
		if (PastPauseDuration.Ticks >= 0)
		{
			return;
		}
		ArgumentException ex = new ArgumentException("Application has got wrong time after being paused", "PastPauseDuration");
		Debug.LogException(ex);
		throw ex;
	}

	protected virtual void OnTimerComplete()
	{
		TimeLeft = TimeSpan.Zero;
		isActive.Value = false;
		update.OnNext(TimeLeft);
		timeIsUp.OnNext(this);
	}

	protected virtual void ActionOnEveryUpdate()
	{
		update.OnNext(TimeLeft);
		TimeLeft -= oneSecond;
	}

	public virtual void Stop()
	{
		timerStream?.Dispose();
		isActive.Value = false;
	}

	public void Clear()
	{
		Stop();
		TimeLeft = TimeSpan.Zero;
	}

	public virtual void Dispose()
	{
		applicationPauseStream.Dispose();
		Stop();
		DisposeSubject<TimeSpan>(update);
		DisposeSubject<GenericTimer>(timeIsUp);
		isActive.Dispose();
	}

	private void DisposeSubject<T>(Subject<T> subject)
	{
		subject.OnCompleted();
		subject?.Dispose();
	}
}
