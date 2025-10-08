using System;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Events;
using Merge.Meta.RoomObjects;
using UniRx;

namespace GreenT.HornyScapes;

public abstract class BaseStateService<T>
{
	protected readonly CalendarQueue _calendarQueue;

	private readonly EventStructureType _eventStructureType;

	public BaseStateService(CalendarQueue calendarQueue, EventStructureType eventStructureType)
	{
		_calendarQueue = calendarQueue;
		_eventStructureType = eventStructureType;
	}

	public IObservable<T> OnStart()
	{
		return from calendar in _calendarQueue.OnConcreteCalendarStateChange(_eventStructureType, EntityStatus.InProgress)
			select GetModel(calendar.BalanceId, calendar.UniqID);
	}

	public IObservable<T> OnEnd()
	{
		return from calendar in _calendarQueue.OnCalendarEnd(_eventStructureType)
			select GetModel(calendar.BalanceId, calendar.UniqID);
	}

	public IObservable<int> OnStartDownloadTuple(ICalendarLoader calendarLoader)
	{
		return from calendar in calendarLoader.OnConcreteCalendarLoadingStateChange(_eventStructureType, CalendarLoadingStatus.Start)
			select calendar.EventMapper.ID;
	}

	public IObservable<(T, CalendarModel)> OnEndDownloadTuple(ICalendarLoader calendarLoader)
	{
		return calendarLoader.OnConcreteCalendarLoadingStateChange(_eventStructureType, CalendarLoadingStatus.End).Select(delegate(CalendarModel calendar)
		{
			T model = GetModel(calendar.BalanceId, calendar.UniqID);
			return (model: model, calendar: calendar);
		});
	}

	public IObservable<(T, CalendarModel)> OnStartTuple()
	{
		return from calendar in _calendarQueue.OnConcreteCalendarStateChange(_eventStructureType, EntityStatus.InProgress)
			select (GetModel(calendar.BalanceId, calendar.UniqID), calendar: calendar);
	}

	public IObservable<(T, CalendarModel)> OnEndTuple()
	{
		return from calendar in _calendarQueue.OnCalendarEnd(_eventStructureType)
			select (GetModel(calendar.BalanceId, calendar.UniqID), calendar: calendar);
	}

	public bool IsAnyInProgress()
	{
		return _calendarQueue.HasInProgressCalendar(_eventStructureType);
	}

	protected abstract T GetModel(int eventId, int calendarId);
}
