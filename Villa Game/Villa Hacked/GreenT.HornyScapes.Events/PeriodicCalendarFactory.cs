using System;
using GreenT.Data;
using StripClub.Model;
using Zenject;

namespace GreenT.HornyScapes.Events;

public class PeriodicCalendarFactory : CalendarBaseFactory<PeriodicCalendarMapper, PeriodicCalendar>
{
	public PeriodicCalendarFactory(IFactory<UnlockType, string, LockerSourceType, ILocker> lockerFactory, ISaver saver, IClock clock, [InjectOptional] TimeInstaller.TimerCollection timers, EventCalendarLoader eventCalendarLoader, BattlePassCalendarLoader battlePassCalendarLoader, MiniEventCalendarLoader miniEventCalendarLoader, SelloutCalendarLoader selloutCalendarLoader, EventStrategyLightWeightFactory eventStrategyLightWeightFactory, BattlePassStrategyLightWeightFactory battlePassStrategyLightWeightFactory, MiniEventStrategyLightWeightFactory miniEventStrategyLightWeightFactory, SelloutStrategyLightWeightFactory selloutStrategyLightWeightFactory)
		: base(lockerFactory, saver, clock, timers, eventCalendarLoader, battlePassCalendarLoader, miniEventCalendarLoader, selloutCalendarLoader, eventStrategyLightWeightFactory, battlePassStrategyLightWeightFactory, miniEventStrategyLightWeightFactory, selloutStrategyLightWeightFactory)
	{
	}

	public override PeriodicCalendar Create(PeriodicCalendarMapper mapper)
	{
		try
		{
			EventStructureType event_type = mapper.event_type;
			ILocker[] locker = CreateLockers(mapper);
			IEventMapper eventInfo = GetEventInfo(event_type, mapper.event_id);
			PeriodicCalendar periodicCalendar = new PeriodicCalendar(mapper.id, event_type, eventInfo, mapper.duration, locker, mapper.show_promo, mapper.start_date, clock, CreateStrategy(event_type), mapper.last_chance_duration);
			periodicCalendar.Initialize();
			timers?.Add(periodicCalendar.Duration);
			timers?.Add(periodicCalendar.StartTimer);
			timers?.Add(periodicCalendar.ComingSoonTimer);
			timers?.Add(periodicCalendar.LastChanceTimer);
			saver.Add(periodicCalendar);
			return periodicCalendar;
		}
		catch (Exception innerException)
		{
			innerException.SendException($"{GetType().Name}: can't create Calendar id = {mapper.event_id}");
			return null;
		}
	}
}
