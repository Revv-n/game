using GreenT.HornyScapes.Analytics;
using StripClub.Model.Data;

namespace GreenT.HornyScapes.MergeStore;

public class Analytics
{
	private readonly IAmplitudeSender<AmplitudeEvent> _amplitude;

	private readonly CurrencyAmplitudeAnalytic _currencyAmplitudeAnalytic;

	public Analytics(IAmplitudeSender<AmplitudeEvent> amplitude, CurrencyAmplitudeAnalytic currencyAmplitudeAnalytic)
	{
		_amplitude = amplitude;
		_currencyAmplitudeAnalytic = currencyAmplitudeAnalytic;
	}

	public void TrackRefresh(Cost cost, StoreSection section)
	{
		_currencyAmplitudeAnalytic.SendSpentEvent(cost.Currency, cost.Amount, CurrencyAmplitudeAnalytic.SourceType.RefreshMergeShop, section.ContentType);
		RefreshAmplitudeEvent analyticsEvent = new RefreshAmplitudeEvent(cost, section);
		_amplitude.AddEvent(analyticsEvent);
	}

	public void BoughtItem(Cost cost, ItemBuyRequest request)
	{
		_currencyAmplitudeAnalytic.SendSpentEvent(cost.Currency, cost.Amount, CurrencyAmplitudeAnalytic.SourceType.MergeShop, request.Item.ContentType);
		ItemShopBoughtAmplitudeEvent analyticsEvent = new ItemShopBoughtAmplitudeEvent(cost, request);
		_amplitude.AddEvent(analyticsEvent);
	}
}
