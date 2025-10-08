using GreenT.HornyScapes.Analytics;

namespace GreenT.HornyScapes;

public sealed class RestoreEnergyView : BaseRestoreEnergyView<EnergyRestore, RestoreEnergyPopup>
{
	protected override CurrencyAmplitudeAnalytic.SourceType AmplitudeSourceType => CurrencyAmplitudeAnalytic.SourceType.MergeRechargeEnergy;
}
