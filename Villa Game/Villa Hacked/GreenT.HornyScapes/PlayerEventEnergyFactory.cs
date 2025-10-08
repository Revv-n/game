using GreenT.Data;
using GreenT.HornyScapes.Constants;
using StripClub.Extensions;

namespace GreenT.HornyScapes;

public sealed class PlayerEventEnergyFactory : BaseEnergyFactory
{
	public PlayerEventEnergyFactory(IClock clock, IConstants<int> intConstants, ISaver saver, EnergyLoadContainer loadContainer)
		: base(clock, intConstants, saver, loadContainer)
	{
		_startEnergyKey = "start_event_energy";
		_timeEnergyRestoreKey = "time_event_energy_restore";
		_amountEnergyRestoreKey = "amount_event_energy_restore";
		_maxEnergyKey = "max_event_energy";
		_saveKey = "EventEnergy";
	}
}
