using GreenT.HornyScapes.Analytics;

namespace GreenT.HornyScapes;

public sealed class RestoreEventEnergyView : BaseRestoreEnergyView<RestoreEventEnergy, RestoreEventEnergyPopup>
{
	protected override CurrencyAmplitudeAnalytic.SourceType AmplitudeSourceType => CurrencyAmplitudeAnalytic.SourceType.MergeRechargeEventEnergy;
}
