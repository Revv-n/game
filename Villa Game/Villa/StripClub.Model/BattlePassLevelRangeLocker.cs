using GreenT.HornyScapes;

namespace StripClub.Model;

public class BattlePassLevelRangeLocker : IntervalLocker
{
	public BattlePassLevelRangeLocker(long from, long to)
		: base(from, to)
	{
	}

	public override void Set(long current)
	{
		isOpen.Value = current >= from && current <= to;
	}
}
