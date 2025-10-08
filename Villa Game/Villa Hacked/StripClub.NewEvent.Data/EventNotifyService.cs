using System;
using System.Collections.Generic;
using GreenT.HornyScapes;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Events;
using UniRx;

namespace StripClub.NewEvent.Data;

public class EventNotifyService : ICalendarQueueListener, IDisposable
{
	private readonly EventProvider _eventProvider;

	private readonly EventSettingsProvider _eventSettingsProvider;

	private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

	public EventNotifyService(EventProvider eventProvider, EventSettingsProvider eventSettingsProvider)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		_eventProvider = eventProvider;
		_eventSettingsProvider = eventSettingsProvider;
	}

	public void Initialize(CalendarQueue calendarQueue)
	{
		_compositeDisposable.Clear();
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<(CalendarModel, Event)>(Observable.Where<(CalendarModel, Event)>(Observable.Select<CalendarModel, (CalendarModel, Event)>(calendarQueue.OnCalendarActiveNotNull(EventStructureType.Event), (Func<CalendarModel, (CalendarModel, Event)>)((CalendarModel calendar) => (calendar: calendar, _eventSettingsProvider.GetEvent(calendar.BalanceId)))), (Func<(CalendarModel, Event), bool>)(((CalendarModel calendar, Event) tuple) => tuple.Item2 != null)), (Action<(CalendarModel, Event)>)_eventProvider.Set), (ICollection<IDisposable>)_compositeDisposable);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Event>(Observable.Where<Event>(Observable.Select<CalendarModel, Event>(Observable.Where<CalendarModel>(calendarQueue.OnCalendarEnd(EventStructureType.Event), (Func<CalendarModel, bool>)((CalendarModel calendar) => calendar != null)), (Func<CalendarModel, Event>)((CalendarModel calendar) => _eventSettingsProvider.GetEvent(calendar.BalanceId))), (Func<Event, bool>)((Event @event) => @event != null && !_eventProvider.CurrentCalendarProperty.Value.Item2.Equals(@event))), (Action<Event>)delegate
		{
			_eventProvider.Reset();
		}), (ICollection<IDisposable>)_compositeDisposable);
	}

	public void Dispose()
	{
		_compositeDisposable.Dispose();
	}
}
