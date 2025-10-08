using Zenject;

namespace GreenT.HornyScapes.Analytics;

public class LinkedContentAnalyticDataFactory : IFactory<CurrencyAmplitudeAnalytic.SourceType, LinkedContentAnalyticData>, IFactory
{
	public LinkedContentAnalyticData Create(CurrencyAmplitudeAnalytic.SourceType param)
	{
		return new LinkedContentAnalyticData(param);
	}
}
