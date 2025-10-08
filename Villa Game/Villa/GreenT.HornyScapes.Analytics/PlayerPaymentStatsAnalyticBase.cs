using UniRx;

namespace GreenT.HornyScapes.Analytics;

public abstract class PlayerPaymentStatsAnalyticBase : BaseAnalytic<decimal>
{
	private ICohortAnalyticConverter cohortAnalyticConverter;

	protected int _previousCohort = -1;

	private readonly PlayerPaymentsStats _playerPaymentsStats;

	public PlayerPaymentStatsAnalyticBase(PlayerPaymentsStats playerPaymentsStats, IAmplitudeSender<AmplitudeEvent> amplitude, ICohortAnalyticConverter cohortAnalyticConverter)
		: base(amplitude)
	{
		_playerPaymentsStats = playerPaymentsStats;
		this.cohortAnalyticConverter = cohortAnalyticConverter;
	}

	public override void Track()
	{
		_playerPaymentsStats.OnCohortPaymentStatsUpdate.Subscribe(base.SendEventIfIsValid).AddTo(onNewStream);
	}

	public override void SendEventByPass(decimal cohort)
	{
		int num = ConvertToCorrectValue(cohort);
		CohortChangeAmplitudeEvent analyticsEvent = new CohortChangeAmplitudeEvent(num);
		_previousCohort = num;
		amplitude.AddEvent(analyticsEvent);
	}

	public int ConvertToCorrectValue(decimal cohort)
	{
		return cohortAnalyticConverter.ConvertToCorrectValue(cohort);
	}

	protected override bool IsValid(decimal cohort)
	{
		return cohortAnalyticConverter.IsValid(cohort, _previousCohort);
	}
}
