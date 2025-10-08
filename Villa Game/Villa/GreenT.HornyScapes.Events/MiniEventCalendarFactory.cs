using GreenT.Data;
using StripClub.Model;
using Zenject;

namespace GreenT.HornyScapes.Events;

public class MiniEventCalendarFactory : PeriodicCalendarFactory
{
	public MiniEventCalendarFactory(IFactory<UnlockType, string, LockerSourceType, ILocker> lockerFactory, ISaver saver, IClock clock, [InjectOptional] TimeInstaller.TimerCollection timers, EventCalendarLoader eventCalendarLoader, BattlePassCalendarLoader battlePassCalendarLoader, MiniEventCalendarLoader miniEventCalendarLoader, SelloutCalendarLoader selloutCalendarLoader, EventStrategyLightWeightFactory eventStrategyLightWeightFactory, BattlePassStrategyLightWeightFactory battlePassStrategyLightWeightFactory, MiniEventStrategyLightWeightFactory miniEventStrategyLightWeightFactory, SelloutStrategyLightWeightFactory selloutStrategyLightWeightFactory)
		: base(lockerFactory, saver, clock, timers, eventCalendarLoader, battlePassCalendarLoader, miniEventCalendarLoader, selloutCalendarLoader, eventStrategyLightWeightFactory, battlePassStrategyLightWeightFactory, miniEventStrategyLightWeightFactory, selloutStrategyLightWeightFactory)
	{
	}
}
