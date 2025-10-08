using UniRx;

namespace GreenT.HornyScapes.BattlePassSpace;

public class BattlePassProvider
{
	private readonly ReactiveProperty<(CalendarModel calendar, BattlePass battlePass)> _calendarChangeProperty = new ReactiveProperty<(CalendarModel, BattlePass)>();

	public IReadOnlyReactiveProperty<(CalendarModel calendar, BattlePass battlePass)> CalendarChangeProperty => _calendarChangeProperty;

	public void Set((CalendarModel calendarModel, BattlePass battlePassSettings) tuple)
	{
		_calendarChangeProperty.Value = tuple;
	}

	public void Reset()
	{
		_calendarChangeProperty.Value = (null, null);
	}
}
