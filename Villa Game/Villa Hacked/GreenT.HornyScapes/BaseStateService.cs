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
		return Observable.Select<CalendarModel, T>(_calendarQueue.OnConcreteCalendarStateChange(_eventStructureType, EntityStatus.InProgress), (Func<CalendarModel, T>)((CalendarModel calendar) => GetModel(calendar.BalanceId, calendar.UniqID)));
	}

	public IObservable<T> OnEnd()
	{
		return Observable.Select<CalendarModel, T>(_calendarQueue.OnCalendarEnd(_eventStructureType), (Func<CalendarModel, T>)((CalendarModel calendar) => GetModel(calendar.BalanceId, calendar.UniqID)));
	}

	public IObservable<int> OnStartDownloadTuple(ICalendarLoader calendarLoader)
	{
		return Observable.Select<CalendarModel, int>(calendarLoader.OnConcreteCalendarLoadingStateChange(_eventStructureType, CalendarLoadingStatus.Start), (Func<CalendarModel, int>)((CalendarModel calendar) => calendar.EventMapper.ID));
	}

	public IObservable<(T, CalendarModel)> OnEndDownloadTuple(ICalendarLoader calendarLoader)
	{
		return Observable.Select<CalendarModel, (T, CalendarModel)>(calendarLoader.OnConcreteCalendarLoadingStateChange(_eventStructureType, CalendarLoadingStatus.End), (Func<CalendarModel, (T, CalendarModel)>)delegate(CalendarModel calendar)
		{
			T model = GetModel(calendar.BalanceId, calendar.UniqID);
			return (model: model, calendar: calendar);
		});
	}

	public IObservable<(T, CalendarModel)> OnStartTuple()
	{
		return Observable.Select<CalendarModel, (T, CalendarModel)>(_calendarQueue.OnConcreteCalendarStateChange(_eventStructureType, EntityStatus.InProgress), (Func<CalendarModel, (T, CalendarModel)>)((CalendarModel calendar) => (GetModel(calendar.BalanceId, calendar.UniqID), calendar: calendar)));
	}

	public IObservable<(T, CalendarModel)> OnEndTuple()
	{
		return Observable.Select<CalendarModel, (T, CalendarModel)>(_calendarQueue.OnCalendarEnd(_eventStructureType), (Func<CalendarModel, (T, CalendarModel)>)((CalendarModel calendar) => (GetModel(calendar.BalanceId, calendar.UniqID), calendar: calendar)));
	}

	public bool IsAnyInProgress()
	{
		return _calendarQueue.HasInProgressCalendar(_eventStructureType);
	}

	protected abstract T GetModel(int eventId, int calendarId);
}
