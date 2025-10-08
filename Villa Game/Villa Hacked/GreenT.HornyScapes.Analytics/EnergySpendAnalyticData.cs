using GreenT.Types;
using StripClub.Model;

namespace GreenT.HornyScapes.Analytics;

public class EnergySpendAnalyticData : BaseEnergyAnalyticData
{
	public ContentType ContentType { get; private set; }

	public EnergySpendAnalyticData(CurrencyType type, int diff, CurrencyAmplitudeAnalytic.SourceType source, CompositeIdentificator compositeIdentificator, ContentType contentType)
		: base(type, diff, source, compositeIdentificator)
	{
		ContentType = contentType;
	}

	public override bool IsComparative(BaseEnergyAnalyticData analyticData)
	{
		EnergySpendAnalyticData energySpendAnalyticData = (EnergySpendAnalyticData)analyticData;
		if (energySpendAnalyticData.Source == base.Source && energySpendAnalyticData.Identificator == base.Identificator)
		{
			return energySpendAnalyticData.ContentType == ContentType;
		}
		return false;
	}
}
