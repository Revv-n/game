using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.External.StripClub._Scripts.MiniEvents;

namespace GreenT.HornyScapes;

public sealed class MiniEventStrategyLightWeightFactory : CalendarStrategyLightWeightFactory<MiniCalendarStrategy, MiniEventDataBuilder, MiniEventCalendarLoader, MiniEventCalendarDispenser, MiniEventDataCleaner>
{
	public MiniEventStrategyLightWeightFactory(MiniEventDataBuilder builder, MiniEventCalendarLoader loader, MiniEventCalendarDispenser dispenser, MiniEventDataCleaner cleaner)
		: base(builder, loader, dispenser, cleaner)
	{
	}

	protected override MiniCalendarStrategy CreateLightWeight()
	{
		return new MiniCalendarStrategy(_builder, _loader, _dispenser, _cleaner);
	}
}
