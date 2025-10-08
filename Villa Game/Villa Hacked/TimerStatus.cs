public struct TimerStatus
{
	public float Delta { get; private set; }

	public float Time { get; private set; }

	public float TimeStart { get; private set; }

	public float TimeEnd { get; private set; }

	public float TimeTotal => TimeEnd - TimeStart;

	public float TimeLeft => TimeEnd - Time;

	public float Percent => Time / TimeTotal;

	public float PercentLeft => 1f - Time / TimeTotal;

	public TimerStatus(float timeEnd)
	{
		this = default(TimerStatus);
		TimeEnd = timeEnd;
	}

	public TimerStatus(float time, float timeEnd)
		: this(timeEnd)
	{
		Time = time;
	}

	public TimerStatus(float time, float timeEnd, float timeStart)
		: this(time, timeEnd)
	{
		TimeStart = timeStart;
	}

	public void SetTime(float newTime)
	{
		Delta = newTime - Time;
		Time = newTime;
	}
}
