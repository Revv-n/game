namespace GreenT.HornyScapes.Analytics;

public class CohortChangeAmplitudeEvent : AmplitudeEvent
{
	private const string EventKey = "cohorts";

	private const string Cohort = "cohort";

	public CohortChangeAmplitudeEvent(int cohort)
		: base("cohorts")
	{
		((AnalyticsEvent)this).AddEventParams("cohort", (object)cohort);
	}
}
