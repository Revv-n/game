using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Events;

namespace GreenT.HornyScapes;

public sealed class BattlePassCalendarStrategy : DefaultCalendarStrategy<BattlePassDataBuilder, BattlePassCalendarLoader, BattlePassCalendarDispenser, BattlePassDataCleaner>
{
	public BattlePassCalendarStrategy(BattlePassDataBuilder builder, BattlePassCalendarLoader loader, BattlePassCalendarDispenser dispenser, BattlePassDataCleaner battlePassDataCleaner)
		: base(builder, loader, dispenser, battlePassDataCleaner)
	{
	}
}
