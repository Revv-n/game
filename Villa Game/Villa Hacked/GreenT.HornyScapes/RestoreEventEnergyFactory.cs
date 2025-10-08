using GreenT.Data;
using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.Constants;
using GreenT.HornyScapes.Content;
using GreenT.HornyScapes.Events;
using StripClub.Model;
using StripClub.Model.Shop;

namespace GreenT.HornyScapes;

public class RestoreEventEnergyFactory : BaseEnergyRestoreFactory<RestoreEventEnergy>
{
	private readonly EventDataCleaner _eventDataCleaner;

	protected override string PriceId => "event_energy_cost";

	protected override string MaxPrice => "event_energy_max_cost";

	protected override string PriceStep => "event_energy_progression";

	protected override string AddEnergy => "event_energy_amount";

	protected override bool IsFreeFirstBuy => false;

	protected override CurrencyAmplitudeAnalytic.SourceType AmplitudeSourceType => CurrencyAmplitudeAnalytic.SourceType.MergeRechargeEventEnergy;

	public RestoreEventEnergyFactory(IPurchaseProcessor purchaseProcessor, CurrencyContentFactory currencyContentFactory, LinkedContentAnalyticDataFactory analyticDataFactory, IConstants<int> initConstants, ISaver saver, IClock clock, EventDataCleaner eventDataCleaner)
		: base(purchaseProcessor, currencyContentFactory, analyticDataFactory, initConstants, saver, clock, CurrencyType.EventEnergy)
	{
		_eventDataCleaner = eventDataCleaner;
	}

	protected override void RestoreInitialization(RestoreEventEnergy energyRestore)
	{
		energyRestore.SetDataCleaner(_eventDataCleaner);
		base.RestoreInitialization(energyRestore);
	}
}
