using GreenT.HornyScapes.Analytics.Bank;
using GreenT.Types;
using StripClub.Model;
using StripClub.Model.Shop;

namespace GreenT.HornyScapes.Analytics;

public class BankAnalytic : BaseEntityAnalytic<Lot>
{
	private readonly CurrencyAmplitudeAnalytic currencyAmplitudeAnalytic;

	public BankAnalytic(IAmplitudeSender<AmplitudeEvent> amplitude, CurrencyAmplitudeAnalytic currencyAmplitudeAnalytic)
		: base(amplitude)
	{
		this.currencyAmplitudeAnalytic = currencyAmplitudeAnalytic;
	}

	protected override bool IsValid(Lot lot)
	{
		if (!(lot is SummonLot))
		{
			return lot is ValuableLot<decimal>;
		}
		return true;
	}

	public override void SendEventByPass(Lot lot)
	{
		if (!(lot is ValuableLot<decimal> bundleLot))
		{
			if (lot is SummonLot summonLot)
			{
				SendSummonEvent(summonLot);
			}
		}
		else
		{
			SendBundleLotEvent(bundleLot);
		}
	}

	private void SendBundleLotEvent(ValuableLot<decimal> bundleLot)
	{
		if (bundleLot.Price.Currency.IsInGameCurrency())
		{
			currencyAmplitudeAnalytic.SendSpentEvent(bundleLot.Price.Currency, decimal.ToInt32(bundleLot.Price.Value), bundleLot.Content.AnalyticData.SourceType, ContentType.Main, bundleLot.Price.CompositeIdentificator);
			LotBoughAmplitudeEvent analyticsEvent = new LotBoughAmplitudeEvent(bundleLot);
			amplitude.AddEvent(analyticsEvent);
		}
	}

	private void SendSummonEvent(SummonLot summonLot)
	{
		if (summonLot.Price.Value > 0)
		{
			currencyAmplitudeAnalytic.SendSpentEvent(summonLot.Price.Currency, summonLot.Price.Value, CurrencyAmplitudeAnalytic.SourceType.Summon, summonLot.TaskType, summonLot.Price.CompositeIdentificator);
		}
		BaseSummonAmplitudeEvent analyticsEvent = new SummonAmplitudeEvent(summonLot);
		amplitude.AddEvent(analyticsEvent);
	}
}
