using System;
using System.Collections.Generic;
using StripClub.UI;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class MiniEventTimerController
{
	private const int RATING_EXTRA_SECONDS = 86400;

	private Dictionary<TimerKey, GenericTimer> _timers;

	public MiniEventTimerController()
	{
		_timers = new Dictionary<TimerKey, GenericTimer>();
	}

	public void StartDefaultTimer(int minieventId, int calendarId, long duration)
	{
		GenericTimer value = new GenericTimer(TimeSpan.FromSeconds(duration));
		TimerKey key = CreateTimerKey(minieventId, calendarId);
		_timers.TryAdd(key, value);
	}

	public void StartRatingTimer(int minieventId, int calendarId, long duration)
	{
		GenericTimer value = new GenericTimer(TimeSpan.FromSeconds(duration - 86400));
		TimerKey key = CreateTimerKey(minieventId, calendarId);
		_timers.TryAdd(key, value);
	}

	public GenericTimer TryGetTimerByID(int minieventId, int calendarId)
	{
		TimerKey key = CreateTimerKey(minieventId, calendarId);
		if (_timers.ContainsKey(key))
		{
			return _timers[key];
		}
		return null;
	}

	private TimerKey CreateTimerKey(int minieventId, int calendarId)
	{
		TimerKey result = default(TimerKey);
		result.MiniEventId = minieventId;
		result.CalendarId = calendarId;
		return result;
	}
}
