using GreenT.Types;

namespace GreenT.HornyScapes.Analytics;

public sealed class RouletteAnalytic : BaseEntityAnalytic<RouletteLot>
{
	private readonly CurrencyAmplitudeAnalytic _currencyAmplitudeAnalytic;

	public RouletteAnalytic(IAmplitudeSender<AmplitudeEvent> amplitude, CurrencyAmplitudeAnalytic currencyAmplitudeAnalytic)
		: base(amplitude)
	{
		_currencyAmplitudeAnalytic = currencyAmplitudeAnalytic;
	}

	protected override bool IsValid(RouletteLot lot)
	{
		if (lot != null)
		{
			return true;
		}
		return false;
	}

	public override void SendEventByPass(RouletteLot rouletteLot)
	{
		SendRouletteEvent(rouletteLot);
	}

	private void SendRouletteEvent(RouletteLot rouletteLot)
	{
		if (rouletteLot.CurrentPrice.Value > 0)
		{
			_currencyAmplitudeAnalytic.SendSpentEvent(rouletteLot.CurrentPrice.Currency, rouletteLot.CurrentPrice.Value, CurrencyAmplitudeAnalytic.SourceType.Roulette, ContentType.Event, rouletteLot.CurrentPrice.CompositeIdentificator);
		}
		if (rouletteLot.Wholesale)
		{
			for (int i = 0; i < rouletteLot.IsMainReward.Count; i++)
			{
				if (rouletteLot.IsMainReward[i])
				{
					AmplitudeEvent analyticsEvent = null;
					if (rouletteLot is RouletteBankSummonLot)
					{
						analyticsEvent = new RouletteMainRewardAmplitudeEvent(rouletteLot.ID, rouletteLot.AttemptsForAnalytics[i], rouletteLot.ShopSource);
					}
					else if (rouletteLot is RouletteSummonLot)
					{
						analyticsEvent = new MiniEventRouletteMainRewardAmplitudeEvent(rouletteLot.ID, rouletteLot.AttemptsForAnalytics[i], rouletteLot.ShopSource);
					}
					amplitude.AddEvent(analyticsEvent);
				}
				else
				{
					AmplitudeEvent analyticsEvent2 = null;
					if (rouletteLot is RouletteBankSummonLot)
					{
						analyticsEvent2 = new RouletteAmplitudeEvent(rouletteLot.ID, rouletteLot.ShopSource);
					}
					else if (rouletteLot is RouletteSummonLot)
					{
						analyticsEvent2 = new MiniEventRouletteAmplitudeEvent(rouletteLot.ID, rouletteLot.ShopSource);
					}
					amplitude.AddEvent(analyticsEvent2);
				}
			}
		}
		else if (rouletteLot.IsLastMainReward)
		{
			AmplitudeEvent analyticsEvent3 = null;
			if (rouletteLot is RouletteBankSummonLot)
			{
				analyticsEvent3 = new RouletteMainRewardAmplitudeEvent(rouletteLot.ID, rouletteLot.Attempts, rouletteLot.ShopSource);
			}
			else if (rouletteLot is RouletteSummonLot)
			{
				analyticsEvent3 = new MiniEventRouletteMainRewardAmplitudeEvent(rouletteLot.ID, rouletteLot.Attempts, rouletteLot.ShopSource);
			}
			amplitude.AddEvent(analyticsEvent3);
		}
		else
		{
			AmplitudeEvent analyticsEvent4 = null;
			if (rouletteLot is RouletteBankSummonLot)
			{
				analyticsEvent4 = new RouletteAmplitudeEvent(rouletteLot.ID, rouletteLot.ShopSource);
			}
			else if (rouletteLot is RouletteSummonLot)
			{
				analyticsEvent4 = new MiniEventRouletteAmplitudeEvent(rouletteLot.ID, rouletteLot.ShopSource);
			}
			amplitude.AddEvent(analyticsEvent4);
		}
	}
}
