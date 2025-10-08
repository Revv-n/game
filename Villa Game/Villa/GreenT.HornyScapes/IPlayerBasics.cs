using GreenT.Types;
using StripClub.Extensions;
using StripClub.Model;
using StripClub.Model.Data;

namespace GreenT.HornyScapes;

public interface IPlayerBasics
{
	Currencies Balance { get; }

	RestorableValue<int> Energy { get; }

	RestorableEventEnergyValue<int> EventEnergy { get; }

	void Init();

	void AddCurrencyType(CurrencyType type, SimpleCurrency currency, CompositeIdentificator compositeIdentificator);
}
