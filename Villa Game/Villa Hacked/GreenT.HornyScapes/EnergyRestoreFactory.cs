using GreenT.Data;
using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.Constants;
using GreenT.HornyScapes.Content;
using StripClub.Model;
using StripClub.Model.Shop;

namespace GreenT.HornyScapes;

public class EnergyRestoreFactory : BaseEnergyRestoreFactory<EnergyRestore>
{
	protected override string PriceId => "energy_cost";

	protected override string MaxPrice => "energy_max_cost";

	protected override string PriceStep => "energy_progression";

	protected override string AddEnergy => "energy_amount";

	protected override bool IsFreeFirstBuy => true;

	protected override CurrencyAmplitudeAnalytic.SourceType AmplitudeSourceType => CurrencyAmplitudeAnalytic.SourceType.MergeRechargeEnergy;

	public EnergyRestoreFactory(IPurchaseProcessor purchaseProcessor, CurrencyContentFactory currencyContentFactory, LinkedContentAnalyticDataFactory analyticDataFactory, IConstants<int> initConstants, ISaver saver, IClock clock)
		: base(purchaseProcessor, currencyContentFactory, analyticDataFactory, initConstants, saver, clock, CurrencyType.Energy)
	{
	}
}
