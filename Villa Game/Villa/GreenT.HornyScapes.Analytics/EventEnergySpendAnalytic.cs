using GreenT.Types;
using StripClub.Model;
using StripClub.Model.Shop;

namespace GreenT.HornyScapes.Analytics;

public class EventEnergySpendAnalytic : EnergySpendAnalyticBase
{
	public EventEnergySpendAnalytic(IAmplitudeSender<AmplitudeEvent> amplitude, CurrencyAmplitudeAnalytic currencyAmplitudeAnalytic, ICurrencyProcessor currencyProcessor)
		: base(amplitude, currencyAmplitudeAnalytic, currencyProcessor, CurrencyType.EventEnergy, ContentType.Event)
	{
	}
}
