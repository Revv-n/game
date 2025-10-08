using GreenT.Types;
using StripClub.Model;

namespace GreenT.HornyScapes.Analytics;

public class EnergyRecievedAnalyticData : BaseEnergyAnalyticData
{
	public EnergyRecievedAnalyticData(CurrencyType type, int diff, CurrencyAmplitudeAnalytic.SourceType source, CompositeIdentificator compositeIdentificator)
		: base(type, diff, source, compositeIdentificator)
	{
	}
}
