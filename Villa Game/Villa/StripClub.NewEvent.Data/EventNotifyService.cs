using System;
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
		_eventProvider = eventProvider;
		_eventSettingsProvider = eventSettingsProvider;
	}

	public void Initialize(CalendarQueue calendarQueue)
	{
		_compositeDisposable.Clear();
		(from calendar in calendarQueue.OnCalendarActiveNotNull(EventStructureType.Event)
			select (calendar: calendar, _eventSettingsProvider.GetEvent(calendar.BalanceId)) into tuple
			where tuple.Item2 != null
			select tuple).Subscribe(_eventProvider.Set).AddTo(_compositeDisposable);
		(from calendar in calendarQueue.OnCalendarEnd(EventStructureType.Event)
			where calendar != null
			select _eventSettingsProvider.GetEvent(calendar.BalanceId) into @event
			where @event != null && !_eventProvider.CurrentCalendarProperty.Value.@event.Equals(@event)
			select @event).Subscribe(delegate
		{
			_eventProvider.Reset();
		}).AddTo(_compositeDisposable);
	}

	public void Dispose()
	{
		_compositeDisposable.Dispose();
	}
}
