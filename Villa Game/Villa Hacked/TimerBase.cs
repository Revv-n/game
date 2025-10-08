using System;

public abstract class TimerBase
{
	private TimerStatus info;

	public RefTimer Timer { get; private set; }

	public float CurrentTime { get; private set; }

	public bool Repeating { get; set; }

	public event Action<TimerStatus> TickCallback;

	public event Action CompleteCallback;

	public TimerBase(RefTimer timer, Action callback, Action<TimerStatus> tickCallback = null)
	{
		Timer = timer;
		CompleteCallback += callback;
		if (tickCallback != null)
		{
			TickCallback += tickCallback;
		}
		info = new TimerStatus(Timer.Passed, Timer.TotalTime);
		StartTimer();
	}

	public void Kill()
	{
		this.CompleteCallback = null;
		this.TickCallback = null;
		StopTimer();
	}

	protected void OnTimerTick(float time)
	{
		info.SetTime(time);
		this.TickCallback?.Invoke(info);
	}

	protected void AtComplete()
	{
		this.CompleteCallback?.Invoke();
		if (Repeating)
		{
			Timer.StartTime = TimeMaster.Now;
			CurrentTime = 0f;
			info = new TimerStatus(0f, Timer.TotalTime);
			StartTimer();
		}
	}

	protected abstract void StartTimer();

	protected abstract void StopTimer();
}
