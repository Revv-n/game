namespace GreenT.HornyScapes.Analytics.Nutaku;

public class PlayerPaymentStatsAnalytic : PlayerPaymentStatsAnalyticBase
{
	public PlayerPaymentStatsAnalytic(PlayerPaymentsStats playerPaymentsStats, IAmplitudeSender<AmplitudeEvent> amplitude, ICohortAnalyticConverter cohortAnalyticConverter)
		: base(playerPaymentsStats, amplitude, cohortAnalyticConverter)
	{
		_previousCohort = -1000;
	}
}
