using GreenT.HornyScapes.BattlePassSpace;

namespace GreenT.HornyScapes.Events;

public class SelloutCalendarStateStrategy : ICalendarStateStrategy
{
	private readonly CalendarModel _context;

	public SelloutCalendarStateStrategy(CalendarModel context)
	{
		_context = context;
	}

	public bool CheckIfRewarded()
	{
		return true;
	}

	public void OnInProgress()
	{
	}

	public void OnComplete()
	{
	}

	public void OnRewarded()
	{
	}
}
