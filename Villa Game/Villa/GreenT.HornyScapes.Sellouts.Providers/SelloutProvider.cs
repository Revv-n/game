using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Sellouts.Models;
using UniRx;

namespace GreenT.HornyScapes.Sellouts.Providers;

public class SelloutProvider
{
	private readonly ReactiveProperty<(CalendarModel calendar, Sellout sellout)> _currentCalendarProperty = new ReactiveProperty<(CalendarModel, Sellout)>();

	public IReadOnlyReactiveProperty<(CalendarModel calendar, Sellout sellout)> CurrentCalendarProperty => _currentCalendarProperty;

	public void Set((CalendarModel calendarModel, Sellout sellout) tuple)
	{
		_currentCalendarProperty.Value = tuple;
	}

	public void Reset()
	{
		_currentCalendarProperty.Value = (null, null);
	}
}
