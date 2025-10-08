using System;
using GreenT.HornyScapes._HornyScapes._Scripts.Events.Calendar;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Events;
using StripClub.NewEvent.Save;

namespace GreenT.HornyScapes;

public class DefaultCalendarStrategy<B, L, D, C> : ICalendarStrategy where B : ICalendarDataBuilder where L : ICalendarLoader where D : ICalendarDispenser where C : ICalendarDataCleaner
{
	private readonly B _builder;

	private readonly L _loader;

	private readonly D _dispenser;

	private readonly C _cleaner;

	protected DefaultCalendarStrategy(B builder, L loader, D dispenser, C cleaner)
	{
		_builder = builder;
		_loader = loader;
		_dispenser = dispenser;
		_cleaner = cleaner;
	}

	public virtual void Build(CalendarModel calendarModel, string saveKey)
	{
		_builder.CreateData(calendarModel, saveKey);
	}

	public virtual IObservable<CalendarModel> Load(CalendarModel calendarModel)
	{
		calendarModel.IsLoading = true;
		return _loader.Load(calendarModel);
	}

	public virtual void Dispense(int balanceId)
	{
		_dispenser.Set(balanceId);
	}

	public void Clean(CalendarModel calendarModel)
	{
		_cleaner.CleanData(calendarModel);
	}

	public IEventMapper GetMapper(int id)
	{
		return _loader.GetEventMapper(id);
	}
}
