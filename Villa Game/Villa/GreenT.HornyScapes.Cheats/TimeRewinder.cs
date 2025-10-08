using System;
using StripClub.Extensions;
using StripClub.UI;

namespace GreenT.HornyScapes.Cheats;

public class TimeRewinder : ITimeRewinder
{
	private ITimeRewinder rewinder;

	private TimeInstaller.TimerCollection timers;

	private TimeHelper timeHelper;

	public TimeSpan TotalRewindedTime { get; private set; } = TimeSpan.Zero;


	public TimeRewinder(ITimeRewinder rewinder, TimeInstaller.TimerCollection timers, TimeHelper timeHelper)
	{
		this.rewinder = rewinder;
		this.timers = timers;
		this.timeHelper = timeHelper;
	}

	public void Rewind(TimeSpan time)
	{
		TotalRewindedTime += time;
		rewinder.Rewind(time);
		RewindGenericTimers(time);
		_ = time.Ticks;
		_ = 0;
	}

	private void RewindGenericTimers(TimeSpan time)
	{
		foreach (GenericTimer item in timers.Collection)
		{
			item.Rewind(time);
		}
	}

	public void Reset()
	{
		rewinder.Reset();
		RewindGenericTimers(-TotalRewindedTime);
		TotalRewindedTime = TimeSpan.Zero;
	}
}
