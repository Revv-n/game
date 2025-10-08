using System;
using System.Runtime.CompilerServices;
using GreenT.Data;
using GreenT.HornyScapes._HornyScapes._Scripts.Events.Calendar;
using StripClub.Model;
using Zenject;

namespace GreenT.HornyScapes.Events;

public abstract class CalendarBaseFactory<TMapper, TEntity> : IFactory<TMapper, TEntity>, IFactory
{
	private readonly EventCalendarLoader _eventCalendarLoader;

	private readonly BattlePassCalendarLoader _battlePassCalendarLoader;

	private readonly MiniEventCalendarLoader _miniEventCalendarLoader;

	private readonly SelloutCalendarLoader _selloutCalendarLoader;

	private readonly EventStrategyLightWeightFactory _eventStrategyLightWeightFactory;

	private readonly BattlePassStrategyLightWeightFactory _battlePassStrategyLightWeightFactory;

	private readonly MiniEventStrategyLightWeightFactory _miniEventStrategyLightWeightFactory;

	private readonly SelloutStrategyLightWeightFactory _selloutStrategyLightWeightFactory;

	protected readonly IFactory<UnlockType, string, LockerSourceType, ILocker> lockerFactory;

	protected readonly ISaver saver;

	protected readonly IClock clock;

	protected readonly TimeInstaller.TimerCollection timers;

	protected CalendarBaseFactory(IFactory<UnlockType, string, LockerSourceType, ILocker> lockerFactory, ISaver saver, IClock clock, [InjectOptional] TimeInstaller.TimerCollection timers, EventCalendarLoader eventCalendarLoader, BattlePassCalendarLoader battlePassCalendarLoader, MiniEventCalendarLoader miniEventCalendarLoader, SelloutCalendarLoader selloutCalendarLoader, EventStrategyLightWeightFactory eventStrategyLightWeightFactory, BattlePassStrategyLightWeightFactory battlePassStrategyLightWeightFactory, MiniEventStrategyLightWeightFactory miniEventStrategyLightWeightFactory, SelloutStrategyLightWeightFactory selloutStrategyLightWeightFactory)
	{
		this.lockerFactory = lockerFactory;
		this.saver = saver;
		this.clock = clock;
		this.timers = timers;
		_eventCalendarLoader = eventCalendarLoader;
		_battlePassCalendarLoader = battlePassCalendarLoader;
		_miniEventCalendarLoader = miniEventCalendarLoader;
		_selloutCalendarLoader = selloutCalendarLoader;
		_eventStrategyLightWeightFactory = eventStrategyLightWeightFactory;
		_battlePassStrategyLightWeightFactory = battlePassStrategyLightWeightFactory;
		_miniEventStrategyLightWeightFactory = miniEventStrategyLightWeightFactory;
		_selloutStrategyLightWeightFactory = selloutStrategyLightWeightFactory;
	}

	public abstract TEntity Create(TMapper mapper);

	protected IEventMapper GetEventInfo(EventStructureType eventStructureType, int event_id)
	{
		return (eventStructureType switch
		{
			EventStructureType.Event => _eventCalendarLoader, 
			EventStructureType.BattlePass => _battlePassCalendarLoader, 
			EventStructureType.Mini => _miniEventCalendarLoader, 
			EventStructureType.Sellout => _selloutCalendarLoader, 
			_ => throw new SwitchExpressionException(eventStructureType), 
		}).GetEventMapper(event_id);
	}

	protected ILocker[] CreateLockers(CalendarMapper mapper)
	{
		LockerExtension lockerExtension = new LockerExtension(lockerFactory);
		ILocker[] array = null;
		try
		{
			return lockerExtension.Create(mapper.unlock_type, mapper.unlock_value, LockerSourceType.Calendar);
		}
		catch (Exception innerException)
		{
			throw innerException.SendException($"{GetType().Name}: calendar id = {mapper.event_id} has error in lockers");
		}
	}

	protected ICalendarStrategy CreateStrategy(EventStructureType eventStructureType)
	{
		return eventStructureType switch
		{
			EventStructureType.Event => _eventStrategyLightWeightFactory.GetLightweightObject<EventCalendarStrategy>(), 
			EventStructureType.BattlePass => _battlePassStrategyLightWeightFactory.GetLightweightObject<BattlePassCalendarStrategy>(), 
			EventStructureType.Mini => _miniEventStrategyLightWeightFactory.GetLightweightObject<MiniCalendarStrategy>(), 
			EventStructureType.Sellout => _selloutStrategyLightWeightFactory.GetLightweightObject<SelloutCalendarStrategy>(), 
			_ => throw new SwitchExpressionException(eventStructureType), 
		};
	}
}
