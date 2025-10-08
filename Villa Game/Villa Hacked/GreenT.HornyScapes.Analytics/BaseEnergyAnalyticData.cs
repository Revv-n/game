using GreenT.Types;
using StripClub.Model;

namespace GreenT.HornyScapes.Analytics;

public abstract class BaseEnergyAnalyticData
{
	public CurrencyType Type { get; private set; }

	public int Diff { get; private set; }

	public CurrencyAmplitudeAnalytic.SourceType Source { get; private set; }

	public CompositeIdentificator Identificator { get; private set; }

	public BaseEnergyAnalyticData(CurrencyType type, int diff, CurrencyAmplitudeAnalytic.SourceType source, CompositeIdentificator compositeIdentificator)
	{
		Type = type;
		Diff = diff;
		Source = source;
		Identificator = compositeIdentificator;
	}

	public void AddDiff(int diff)
	{
		Diff += diff;
	}

	public virtual bool IsComparative(BaseEnergyAnalyticData energyRecievedAnalyticData)
	{
		if (energyRecievedAnalyticData.Source == Source)
		{
			return energyRecievedAnalyticData.Identificator == Identificator;
		}
		return false;
	}
}
