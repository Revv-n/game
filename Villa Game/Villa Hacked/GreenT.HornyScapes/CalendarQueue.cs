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

	public IObservable<Unit> OnForcePush => Observable.AsObservable<Unit>((IObservable<Unit>)forcePushQueueSubject);

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
		IDisposable value = ObservableExtensions.Subscribe<EntityStatus>((IObservable<EntityStatus>)calendar.CalendarState, (Action<EntityStatus>)delegate(EntityStatus state)
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
		return Observable.Select<(CalendarModel, EntityStatus), bool>((IObservable<(CalendarModel, EntityStatus)>)_onStateChangeSubject, (Func<(CalendarModel, EntityStatus), bool>)(((CalendarModel, EntityStatus) tuple) => tuple.Item1.EventType == type && tuple.Item2 == state));
	}

	public bool IsCalendarActive(CalendarModel calendar)
	{
		return _calendars.Contains(calendar);
	}

	public IObservable<Unit> OnCalendarCountChange()
	{
		return Observable.AsUnitObservable<CalendarModel>(Observable.Merge<CalendarModel>((IObservable<CalendarModel>)_onAddSubject, new IObservable<CalendarModel>[1] { (IObservable<CalendarModel>)_onRemoveSubject }));
	}

	public IObservable<CalendarModel> OnCalendarActive()
	{
		return (IObservable<CalendarModel>)_onAddSubject;
	}

	public IObservable<CalendarModel> OnCalendarActive(EventStructureType type)
	{
		return Observable.Where<CalendarModel>(OnCalendarActive(), (Func<CalendarModel, bool>)((CalendarModel calendar) => calendar.EventType == type));
	}

	public IObservable<CalendarModel> OnCalendarActiveNotNull()
	{
		return Observable.Where<CalendarModel>(OnCalendarActive(), (Func<CalendarModel, bool>)((CalendarModel calendar) => calendar != null));
	}

	public IObservable<CalendarModel> OnCalendarEnd()
	{
		return (IObservable<CalendarModel>)_onRemoveSubject;
	}

	public IObservable<CalendarModel> OnCalendarEnd(EventStructureType type)
	{
		return Observable.Where<CalendarModel>((IObservable<CalendarModel>)_onRemoveSubject, (Func<CalendarModel, bool>)((CalendarModel calendar) => calendar.EventType == type));
	}

	public IObservable<CalendarModel> OnCalendarActiveNotNull(EventStructureType type)
	{
		return Observable.Where<CalendarModel>(OnCalendarActive(type), (Func<CalendarModel, bool>)((CalendarModel calendar) => calendar != null));
	}

	public IObservable<CalendarModel> OnCalendarAvailable(CalendarModel calendar)
	{
		return Observable.Select<bool, CalendarModel>(Observable.Where<bool>((IObservable<bool>)calendar.IsUnlockedAndNotPassed, (Func<bool, bool>)((bool x) => x)), (Func<bool, CalendarModel>)((bool _) => calendar));
	}

	public IObservable<CalendarModel> OnCalendarStateChange(EventStructureType type)
	{
		return Observable.Select<(CalendarModel, EntityStatus), CalendarModel>(Observable.Where<(CalendarModel, EntityStatus)>((IObservable<(CalendarModel, EntityStatus)>)_onStateChangeSubject, (Func<(CalendarModel, EntityStatus), bool>)(((CalendarModel, EntityStatus) tuple) => tuple.Item1.EventType == type)), (Func<(CalendarModel, EntityStatus), CalendarModel>)(((CalendarModel, EntityStatus) tuple) => tuple.Item1));
	}

	public IObservable<CalendarModel> OnConcreteCalendarStateChange(EventStructureType type, EntityStatus state)
	{
		return Observable.Select<(CalendarModel, EntityStatus), CalendarModel>(Observable.Where<(CalendarModel, EntityStatus)>((IObservable<(CalendarModel, EntityStatus)>)_onStateChangeSubject, (Func<(CalendarModel, EntityStatus), bool>)(((CalendarModel, EntityStatus) tuple) => tuple.Item1.EventType == type && tuple.Item2 == state)), (Func<(CalendarModel, EntityStatus), CalendarModel>)(((CalendarModel, EntityStatus) tuple) => tuple.Item1));
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
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		forcePushQueueSubject.OnNext(default(Unit));
	}
}
