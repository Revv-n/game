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
					AmplitudeEvent amplitudeEvent = null;
					if (rouletteLot is RouletteBankSummonLot)
					{
						amplitudeEvent = new RouletteMainRewardAmplitudeEvent(rouletteLot.ID, rouletteLot.AttemptsForAnalytics[i], rouletteLot.ShopSource);
					}
					else if (rouletteLot is RouletteSummonLot)
					{
						amplitudeEvent = new MiniEventRouletteMainRewardAmplitudeEvent(rouletteLot.ID, rouletteLot.AttemptsForAnalytics[i], rouletteLot.ShopSource);
					}
					((IAnalyticSender<AmplitudeEvent>)(object)amplitude).AddEvent(amplitudeEvent);
				}
				else
				{
					AmplitudeEvent amplitudeEvent2 = null;
					if (rouletteLot is RouletteBankSummonLot)
					{
						amplitudeEvent2 = new RouletteAmplitudeEvent(rouletteLot.ID, rouletteLot.ShopSource);
					}
					else if (rouletteLot is RouletteSummonLot)
					{
						amplitudeEvent2 = new MiniEventRouletteAmplitudeEvent(rouletteLot.ID, rouletteLot.ShopSource);
					}
					((IAnalyticSender<AmplitudeEvent>)(object)amplitude).AddEvent(amplitudeEvent2);
				}
			}
		}
		else if (rouletteLot.IsLastMainReward)
		{
			AmplitudeEvent amplitudeEvent3 = null;
			if (rouletteLot is RouletteBankSummonLot)
			{
				amplitudeEvent3 = new RouletteMainRewardAmplitudeEvent(rouletteLot.ID, rouletteLot.Attempts, rouletteLot.ShopSource);
			}
			else if (rouletteLot is RouletteSummonLot)
			{
				amplitudeEvent3 = new MiniEventRouletteMainRewardAmplitudeEvent(rouletteLot.ID, rouletteLot.Attempts, rouletteLot.ShopSource);
			}
			((IAnalyticSender<AmplitudeEvent>)(object)amplitude).AddEvent(amplitudeEvent3);
		}
		else
		{
			AmplitudeEvent amplitudeEvent4 = null;
			if (rouletteLot is RouletteBankSummonLot)
			{
				amplitudeEvent4 = new RouletteAmplitudeEvent(rouletteLot.ID, rouletteLot.ShopSource);
			}
			else if (rouletteLot is RouletteSummonLot)
			{
				amplitudeEvent4 = new MiniEventRouletteAmplitudeEvent(rouletteLot.ID, rouletteLot.ShopSource);
			}
			((IAnalyticSender<AmplitudeEvent>)(object)amplitude).AddEvent(amplitudeEvent4);
		}
	}
}
