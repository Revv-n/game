using System;
using System.Globalization;
using GreenT.HornyScapes.Events;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.BattlePassSpace;

public abstract class BaseEventStateAnalytic<T> : IInitializable, IDisposable
{
	private readonly EventAnalytic _eventAnalytic;

	private readonly BaseStateService<T> _eventStateService;

	private readonly ICalendarLoader _calendarLoader;

	private IDisposable _eventStartDownload;

	private IDisposable _startTracker;

	private IDisposable _endTracker;

	public BaseEventStateAnalytic(BaseStateService<T> eventStateService, EventAnalytic eventAnalytic, ICalendarLoader calendarLoader)
	{
		_eventStateService = eventStateService;
		_eventAnalytic = eventAnalytic;
		_calendarLoader = calendarLoader;
	}

	public void Initialize()
	{
		_eventStartDownload = _eventStateService.OnStartDownloadTuple(_calendarLoader).Subscribe(OnEventStartDownload);
		_startTracker = _eventStateService.OnEndDownloadTuple(_calendarLoader).Subscribe(OnEventDownloaded);
		_endTracker = _eventStateService.OnEndTuple().Subscribe(OnEventEnd);
	}

	public void Dispose()
	{
		_eventStartDownload.Dispose();
		_startTracker.Dispose();
		_endTracker.Dispose();
	}

	private void OnEventStartDownload(int id)
	{
		StopwatchMaster.Start(id.ToString());
	}

	private void OnEventDownloaded((T eventData, CalendarModel calendar) tuple)
	{
		if (tuple.eventData != null)
		{
			TimeSpan timeSpan = StopwatchMaster.Stop(tuple.calendar.BalanceId.ToString());
			_eventAnalytic.SendEventDownloadEvent(tuple.calendar.EventType, tuple.calendar.BalanceId, tuple.calendar.UniqID, timeSpan.TotalSeconds.ToString("0.0000000", CultureInfo.InvariantCulture));
		}
	}

	private void OnEventEnd((T eventData, CalendarModel calendar) tuple)
	{
		if (tuple.eventData != null)
		{
			_eventAnalytic.SendEventEndEvent(tuple.calendar.EventType, tuple.calendar.BalanceId, tuple.calendar.UniqID, tuple.calendar.DurationTime);
		}
	}
}
