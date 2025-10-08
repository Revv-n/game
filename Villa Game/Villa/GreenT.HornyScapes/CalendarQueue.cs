using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Events;
using Merge.Meta.RoomObjects;
using UniRx;

namespace GreenT.HornyScapes;

public class CalendarQueue : IEnumerable<CalendarModel>, IEnumerable
{
	private readonly List<CalendarModel> _calendars = new List<CalendarModel>();

	private readonly Subject<CalendarModel> _onAddSubject = new Subject<CalendarModel>();

	private readonly Subject<CalendarModel> _onRemoveSubject = new Subject<CalendarModel>();

	private readonly Subject<(CalendarModel, EntityStatus)> _onStateChangeSubject = new Subject<(CalendarModel, EntityStatus)>();

	private readonly Dictionary<CalendarModel, IDisposable> _stateMap = new Dictionary<CalendarModel, IDisposable>();

	private Subject<Unit> forcePushQueueSubject = new Subject<Unit>();

	public IObservable<Unit> OnForcePush => forcePushQueueSubject.AsObservable();

	public void Add(CalendarModel calendar)
	{
		if (calendar == null)
		{
			throw new ArgumentException("Attempt add null calendar");
		}
		if (!_calendars.All((CalendarModel calendarModel) => calendarModel.UniqID != calendar.UniqID))
		{
			throw new ArgumentException($"Attempt activate already active calendar {calendar.UniqID}!");
		}
		_calendars.Add(calendar);
		_onAddSubject.OnNext(calendar);
		IDisposable value = calendar.CalendarState.Subscribe(delegate(EntityStatus state)
		{
			_onStateChangeSubject.OnNext((calendar, state));
		});
		_stateMap.Add(calendar, value);
	}

	public void Remove(CalendarModel calendar)
	{
		if (calendar == null)
		{
			throw new ArgumentException("Attempt end null calendar");
		}
		if (!_calendars.Contains(calendar))
		{
			throw new ArgumentException($"Attempt end inactive calendar {calendar.UniqID}!");
		}
		_calendars.Remove(calendar);
		_onRemoveSubject.OnNext(calendar);
		_stateMap[calendar]?.Dispose();
		_stateMap.Remove(calendar);
	}

	public CalendarModel GetActiveCalendar(EventStructureType type)
	{
		return _calendars.FirstOrDefault((CalendarModel calendar) => calendar.EventType == type);
	}

	public IEnumerable<CalendarModel> GetAllActiveCalendars(EventStructureType type)
	{
		return _calendars.Where((CalendarModel calendar) => calendar.EventType == type);
	}

	public IEnumerable<CalendarModel> GetAllActiveInProgressCalendars(EventStructureType type)
	{
		return _calendars.Where((CalendarModel calendar) => calendar.EventType == type && calendar.CalendarState.Value == EntityStatus.InProgress);
	}

	public bool HasActiveCalendar()
	{
		return _calendars.Any();
	}

	public bool HasActiveCalendar(EventStructureType type)
	{
		return _calendars.Any((CalendarModel calendar) => calendar.EventType == type);
	}

	public bool HasInProgressCalendar(EventStructureType type)
	{
		return _calendars.Any((CalendarModel calendar) => calendar.EventType == type && calendar.CalendarState.Value == EntityStatus.InProgress);
	}

	public IObservable<bool> OnCalendarStateChange(EventStructureType type, EntityStatus state)
	{
		return _onStateChangeSubject.Select(((CalendarModel, EntityStatus) tuple) => tuple.Item1.EventType == type && tuple.Item2 == state);
	}

	public bool IsCalendarActive(CalendarModel calendar)
	{
		return _calendars.Contains(calendar);
	}

	public IObservable<Unit> OnCalendarCountChange()
	{
		return _onAddSubject.Merge(_onRemoveSubject).AsUnitObservable();
	}

	public IObservable<CalendarModel> OnCalendarActive()
	{
		return _onAddSubject;
	}

	public IObservable<CalendarModel> OnCalendarActive(EventStructureType type)
	{
		return from calendar in OnCalendarActive()
			where calendar.EventType == type
			select calendar;
	}

	public IObservable<CalendarModel> OnCalendarActiveNotNull()
	{
		return from calendar in OnCalendarActive()
			where calendar != null
			select calendar;
	}

	public IObservable<CalendarModel> OnCalendarEnd()
	{
		return _onRemoveSubject;
	}

	public IObservable<CalendarModel> OnCalendarEnd(EventStructureType type)
	{
		return _onRemoveSubject.Where((CalendarModel calendar) => calendar.EventType == type);
	}

	public IObservable<CalendarModel> OnCalendarActiveNotNull(EventStructureType type)
	{
		return from calendar in OnCalendarActive(type)
			where calendar != null
			select calendar;
	}

	public IObservable<CalendarModel> OnCalendarAvailable(CalendarModel calendar)
	{
		return from x in calendar.IsUnlockedAndNotPassed
			where x
			select x into _
			select calendar;
	}

	public IObservable<CalendarModel> OnCalendarStateChange(EventStructureType type)
	{
		return from tuple in _onStateChangeSubject
			where tuple.Item1.EventType == type
			select tuple.Item1;
	}

	public IObservable<CalendarModel> OnConcreteCalendarStateChange(EventStructureType type, EntityStatus state)
	{
		return from tuple in _onStateChangeSubject
			where tuple.Item1.EventType == type && tuple.Item2 == state
			select tuple.Item1;
	}

	public IEnumerator<CalendarModel> GetEnumerator()
	{
		return _calendars.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public void PushNextCalendar()
	{
		forcePushQueueSubject.OnNext(default(Unit));
	}
}
