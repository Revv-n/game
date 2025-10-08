namespace GreenT.HornyScapes.Analytics;

public abstract class CohortAnalyticConverterBase
{
	public abstract int ConvertToCorrectValue(decimal cohort);

	public abstract bool IsValid(decimal cohort, int previousCohort);
}
