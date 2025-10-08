using System;
using UniRx;
using UnityEngine;

namespace StripClub.UI;

public abstract class MonoTimer : MonoBehaviour
{
	protected Func<TimeSpan, string> timerFormat;

	private CompositeDisposable updateStream = new CompositeDisposable();

	private bool internalTimer;

	public GenericTimer Timer { get; protected set; }

	private void Awake()
	{
		CreateTimerIfNull();
	}

	public virtual void Init(TimeSpan timeLeft, Func<TimeSpan, string> timerFormat)
	{
		CreateTimerIfNull();
		InnerInit(timerFormat);
		Timer.Start(timeLeft);
	}

	public virtual void Init(GenericTimer timer, Func<TimeSpan, string> timerFormat)
	{
		if (internalTimer)
		{
			Timer.Dispose();
		}
		Timer = timer;
		internalTimer = false;
		InnerInit(timerFormat);
	}

	protected virtual void InnerInit(Func<TimeSpan, string> timerFormat)
	{
		SetupDisplayFormat(timerFormat);
		updateStream.Clear();
		if (base.isActiveAndEnabled)
		{
			TimerSubscriptions();
		}
	}

	public void CreateTimerIfNull()
	{
		if (Timer == null)
		{
			Timer = new GenericTimer();
			internalTimer = true;
		}
	}

	private void TimerSubscriptions()
	{
		Timer.OnUpdate.Subscribe(OnEveryUpdate).AddTo(updateStream);
		Timer.OnTimeIsUp.Subscribe(OnTimeIsUp).AddTo(updateStream);
		Timer.IsActive.Subscribe(OnTimerSwitchState).AddTo(updateStream);
	}

	protected virtual void OnTimeIsUp(GenericTimer obj)
	{
	}

	public void SetupDisplayFormat(Func<TimeSpan, string> timerFormat)
	{
		this.timerFormat = timerFormat;
	}

	protected abstract void OnEveryUpdate(TimeSpan timeLeft);

	protected virtual void OnTimerSwitchState(bool isActive)
	{
	}

	protected virtual void OnEnable()
	{
		if (timerFormat != null)
		{
			TimerSubscriptions();
		}
	}

	protected virtual void OnDisable()
	{
		updateStream.Clear();
	}

	protected virtual void OnDestroy()
	{
		updateStream.Dispose();
		if (internalTimer)
		{
			Timer.Dispose();
		}
	}
}
