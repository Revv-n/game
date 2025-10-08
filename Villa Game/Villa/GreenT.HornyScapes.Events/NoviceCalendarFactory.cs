using System;
using GreenT.Data;
using StripClub.Model;
using Zenject;

namespace GreenT.HornyScapes.Events;

public class NoviceCalendarFactory : CalendarBaseFactory<NoviceCalendarMapper, NoviceCalendar>
{
	public NoviceCalendarFactory(IFactory<UnlockType, string, LockerSourceType, ILocker> lockerFactory, ISaver saver, IClock clock, [InjectOptional] TimeInstaller.TimerCollection timers, EventCalendarLoader eventCalendarLoader, BattlePassCalendarLoader battlePassCalendarLoader, MiniEventCalendarLoader miniEventCalendarLoader, SelloutCalendarLoader selloutCalendarLoader, EventStrategyLightWeightFactory eventStrategyLightWeightFactory, BattlePassStrategyLightWeightFactory battlePassStrategyLightWeightFactory, MiniEventStrategyLightWeightFactory miniEventStrategyLightWeightFactory, SelloutStrategyLightWeightFactory selloutStrategyLightWeightFactory)
		: base(lockerFactory, saver, clock, timers, eventCalendarLoader, battlePassCalendarLoader, miniEventCalendarLoader, selloutCalendarLoader, eventStrategyLightWeightFactory, battlePassStrategyLightWeightFactory, miniEventStrategyLightWeightFactory, selloutStrategyLightWeightFactory)
	{
	}

	public override NoviceCalendar Create(NoviceCalendarMapper mapper)
	{
		try
		{
			EventStructureType event_type = mapper.event_type;
			ILocker[] locker = CreateLockers(mapper);
			IEventMapper eventInfo = GetEventInfo(event_type, mapper.event_id);
			NoviceCalendar noviceCalendar = new NoviceCalendar(event_type, eventInfo, mapper.duration, locker, mapper.id, clock, CreateStrategy(event_type), mapper.last_chance_duration);
			noviceCalendar.Initialize();
			timers?.Add(noviceCalendar.ComingSoonTimer);
			timers?.Add(noviceCalendar.Duration);
			timers?.Add(noviceCalendar.LastChanceTimer);
			saver.Add(noviceCalendar);
			return noviceCalendar;
		}
		catch (Exception innerException)
		{
			throw innerException.SendException($"{GetType().Name}: can't create Calendar id = {mapper.event_id}");
		}
	}
}
