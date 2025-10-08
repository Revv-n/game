using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.Sellouts.Services;

namespace GreenT.HornyScapes;

public sealed class SelloutCalendarStrategy : DefaultCalendarStrategy<SelloutDataBuilder, SelloutCalendarLoader, SelloutCalendarDispenser, SelloutDataCleaner>
{
	public SelloutCalendarStrategy(SelloutDataBuilder builder, SelloutCalendarLoader loader, SelloutCalendarDispenser dispenser, SelloutDataCleaner cleaner)
		: base(builder, loader, dispenser, cleaner)
	{
	}
}
