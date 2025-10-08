using System;

namespace GreenT.HornyScapes.Analytics;

[Serializable]
public class LinkedContentAnalyticData
{
	public CurrencyAmplitudeAnalytic.SourceType SourceType;

	public LinkedContentAnalyticData(CurrencyAmplitudeAnalytic.SourceType sourceType)
	{
		SourceType = sourceType;
	}
}
