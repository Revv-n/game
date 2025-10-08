using System;

namespace GreenT.HornyScapes;

public class LocalClock : IClock, ITimeRewinder
{
	public TimeSpan GlobalOffset = DateTime.UtcNow - DateTime.Now;

	public TimeSpan skipTime = TimeSpan.Zero;

	public DateTime GetTime()
	{
		return DateTime.UtcNow + skipTime;
	}

	public DateTime GetDate()
	{
		return GetTime().Date;
	}

	public TimeSpan GetTimeEndDay()
	{
		DateTime time = GetTime();
		return time.AddDays(1.0).Date.Subtract(time);
	}

	void ITimeRewinder.Rewind(TimeSpan time)
	{
		skipTime += time;
	}

	public void Reset()
	{
		skipTime = TimeSpan.Zero;
	}
}
