namespace GreenT.HornyScapes.Analytics;

public class CohortChangeAmplitudeEvent : AmplitudeEvent
{
	private const string EventKey = "cohorts";

	private const string Cohort = "cohort";

	public CohortChangeAmplitudeEvent(int cohort)
		: base("cohorts")
	{
		AddEventParams("cohort", cohort);
	}
}
