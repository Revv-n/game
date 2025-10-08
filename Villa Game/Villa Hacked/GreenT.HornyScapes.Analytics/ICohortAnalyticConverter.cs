namespace GreenT.HornyScapes.Analytics;

public interface ICohortAnalyticConverter
{
	int ConvertToCorrectValue(decimal cohort);

	bool IsValid(decimal cohort, int previousCohort);
}
