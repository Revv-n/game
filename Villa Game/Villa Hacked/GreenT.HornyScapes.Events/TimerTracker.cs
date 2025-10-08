using System;
using StripClub.Extensions;
using StripClub.UI;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Events;

public class TimerTracker
{
	private readonly IClock _clock;

	private readonly TimeInstaller.TimerCollection _timerCollection;

	public TimerTracker([InjectOptional] TimeInstaller.TimerCollection timerCollection, IClock clock)
	{
		_clock = clock;
		_timerCollection = timerCollection;
	}

	public GenericTimer AddTrackTimer(long timestamp)
	{
		GenericTimer genericTimer = new GenericTimer();
		long num = _clock.GetTime().ConvertToUnixTimestamp();
		long num2 = timestamp - num;
		if (num2 < 0)
		{
			num2 = 0L;
		}
		genericTimer.Start(TimeSpan.FromSeconds(num2));
		_timerCollection.Add(genericTimer);
		return genericTimer;
	}

	public IObservable<long> AddTrackTime(long timestamp)
	{
		long num = _clock.GetTime().ConvertToUnixTimestamp();
		long num2 = timestamp - num;
		if (num2 < 0)
		{
			num2 = 0L;
		}
		return Observable.Timer(TimeSpan.FromSeconds(num2), Scheduler.MainThreadIgnoreTimeScale);
	}
}
