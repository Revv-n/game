using GreenT.HornyScapes.BattlePassSpace;

namespace GreenT.HornyScapes.Events;

public class BattlePassCalendarStateStrategy : ICalendarStateStrategy
{
	private readonly CalendarModel _context;

	public BattlePassCalendarStateStrategy(CalendarModel context)
	{
		_context = context;
	}

	public bool CheckIfRewarded()
	{
		return !_context.RewardHolder.HasRewards();
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
