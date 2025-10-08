using GreenT.Data;
using GreenT.HornyScapes.Constants;
using StripClub.Extensions;

namespace GreenT.HornyScapes;

public sealed class PlayerEnergyFactory : BaseEnergyFactory
{
	public PlayerEnergyFactory(IClock clock, IConstants<int> intConstants, ISaver saver, EnergyLoadContainer loadContainer)
		: base(clock, intConstants, saver, loadContainer)
	{
		_startEnergyKey = "start_energy";
		_timeEnergyRestoreKey = "time_energy_restore";
		_amountEnergyRestoreKey = "amount_energy_restore";
		_maxEnergyKey = "max_energy";
		_saveKey = "Energy";
	}
}
