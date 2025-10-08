using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Events;
using UniRx;

namespace StripClub.NewEvent.Data;

public class EventProvider
{
	private readonly ReactiveProperty<(CalendarModel calendar, Event @event)> _currentCalendarProperty = new ReactiveProperty<(CalendarModel, Event)>();

	public IReadOnlyReactiveProperty<(CalendarModel calendar, Event @event)> CurrentCalendarProperty => (IReadOnlyReactiveProperty<(CalendarModel calendar, Event @event)>)(object)_currentCalendarProperty;

	public void Set((CalendarModel calendarModel, Event @event) tuple)
	{
		_currentCalendarProperty.Value = tuple;
	}

	public void Reset()
	{
		_currentCalendarProperty.Value = (null, null);
	}
}
