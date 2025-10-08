using GreenT.Types;
using StripClub.Model;
using StripClub.Model.Shop;

namespace GreenT.HornyScapes.Analytics;

public class EnergySpendAnalytic : EnergySpendAnalyticBase
{
	public EnergySpendAnalytic(IAmplitudeSender<AmplitudeEvent> amplitude, CurrencyAmplitudeAnalytic currencyAmplitudeAnalytic, ICurrencyProcessor currencyProcessor)
		: base(amplitude, currencyAmplitudeAnalytic, currencyProcessor, CurrencyType.Energy, ContentType.Main)
	{
	}
}
