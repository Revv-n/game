using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Events;

namespace GreenT.HornyScapes;

public sealed class BattlePassStrategyLightWeightFactory : CalendarStrategyLightWeightFactory<BattlePassCalendarStrategy, BattlePassDataBuilder, BattlePassCalendarLoader, BattlePassCalendarDispenser, BattlePassDataCleaner>
{
	public BattlePassStrategyLightWeightFactory(BattlePassDataBuilder builder, BattlePassCalendarLoader loader, BattlePassCalendarDispenser dispenser, BattlePassDataCleaner battlePassDataCleaner)
		: base(builder, loader, dispenser, battlePassDataCleaner)
	{
	}

	protected override BattlePassCalendarStrategy CreateLightWeight()
	{
		return new BattlePassCalendarStrategy(_builder, _loader, _dispenser, _cleaner);
	}
}
