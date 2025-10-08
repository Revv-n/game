namespace GreenT.HornyScapes.Analytics;

public class PlayerPaymentStatsAnalytic : PlayerPaymentStatsAnalyticBase
{
	public PlayerPaymentStatsAnalytic(PlayerPaymentsStats playerPaymentsStats, IAmplitudeSender<AmplitudeEvent> amplitude, ICohortAnalyticConverter cohortAnalyticConverter)
		: base(playerPaymentsStats, amplitude, cohortAnalyticConverter)
	{
	}
}
