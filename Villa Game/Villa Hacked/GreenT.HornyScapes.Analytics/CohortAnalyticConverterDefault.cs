using UnityEngine;

namespace GreenT.HornyScapes.Analytics;

public class CohortAnalyticConverterDefault : ICohortAnalyticConverter
{
	public int ConvertToCorrectValue(decimal cohort)
	{
		return Mathf.FloorToInt((float)cohort);
	}

	public bool IsValid(decimal cohort, int previousCohort)
	{
		int num = Mathf.FloorToInt((float)cohort);
		return previousCohort != num;
	}
}
