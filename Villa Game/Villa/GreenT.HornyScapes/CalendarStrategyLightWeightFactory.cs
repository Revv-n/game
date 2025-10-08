using GreenT.HornyScapes.Events;
using StripClub.NewEvent.Save;

namespace GreenT.HornyScapes;

public abstract class CalendarStrategyLightWeightFactory<T, B, L, D, C> : LightWeightFactory<T> where B : ICalendarDataBuilder where L : ICalendarLoader where D : ICalendarDispenser where C : ICalendarDataCleaner
{
	protected readonly B _builder;

	protected readonly L _loader;

	protected readonly D _dispenser;

	protected readonly C _cleaner;

	protected CalendarStrategyLightWeightFactory(B builder, L loader, D dispenser, C cleaner)
	{
		_builder = builder;
		_loader = loader;
		_dispenser = dispenser;
		_cleaner = cleaner;
	}
}
