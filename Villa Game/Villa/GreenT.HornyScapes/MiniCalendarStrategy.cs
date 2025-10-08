using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.External.StripClub._Scripts.MiniEvents;

namespace GreenT.HornyScapes;

public sealed class MiniCalendarStrategy : DefaultCalendarStrategy<MiniEventDataBuilder, MiniEventCalendarLoader, MiniEventCalendarDispenser, MiniEventDataCleaner>
{
	public MiniCalendarStrategy(MiniEventDataBuilder builder, MiniEventCalendarLoader loader, MiniEventCalendarDispenser dispenser, MiniEventDataCleaner cleaner)
		: base(builder, loader, dispenser, cleaner)
	{
	}
}
