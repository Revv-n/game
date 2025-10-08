using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.Sellouts.Services;

namespace GreenT.HornyScapes;

public sealed class SelloutStrategyLightWeightFactory : CalendarStrategyLightWeightFactory<SelloutCalendarStrategy, SelloutDataBuilder, SelloutCalendarLoader, SelloutCalendarDispenser, SelloutDataCleaner>
{
	public SelloutStrategyLightWeightFactory(SelloutDataBuilder builder, SelloutCalendarLoader loader, SelloutCalendarDispenser dispenser, SelloutDataCleaner cleaner)
		: base(builder, loader, dispenser, cleaner)
	{
	}

	protected override SelloutCalendarStrategy CreateLightWeight()
	{
		return new SelloutCalendarStrategy(_builder, _loader, _dispenser, _cleaner);
	}
}
