using UnityEngine;

namespace GreenT.HornyScapes.Analytics;

public class CohortAnalyticConverterNutaku : ICohortAnalyticConverter
{
	public int ConvertToCorrectValue(decimal cohort)
	{
		return Mathf.FloorToInt((float)cohort / 100f) * 100;
	}

	public bool IsValid(decimal cohort, int previousCohort)
	{
		int num = Mathf.FloorToInt((float)cohort / 100f) * 100;
		if (num > previousCohort - 100)
		{
			return previousCohort + 100 <= num;
		}
		return true;
	}
}
