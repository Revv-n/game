using GreenT.HornyScapes.Analytics;
using GreenT.Types;
using StripClub.Model;

namespace GreenT.HornyScapes.BannerSpace;

public class Analytics
{
	private readonly IAmplitudeSender<AmplitudeEvent> _amplitude;

	private readonly CurrencyAmplitudeAnalytic _currencyAmplitudeAnalytic;

	public Analytics(IAmplitudeSender<AmplitudeEvent> amplitude, CurrencyAmplitudeAnalytic currencyAmplitudeAnalytic)
	{
		_amplitude = amplitude;
		_currencyAmplitudeAnalytic = currencyAmplitudeAnalytic;
	}

	public void TrackBannerReward(int lootboxId, int garantId, int garantIdStep, int garantChance, CurrencyType currencyType, int amount, ContentType contentType)
	{
		CurrencyAmplitudeAnalytic.SourceType sourceType = CurrencyAmplitudeAnalytic.SourceType.Banner;
		_currencyAmplitudeAnalytic.SendSpentEvent(currencyType, amount, sourceType, contentType);
		BannerRewardAmplitudeEvent bannerRewardAmplitudeEvent = new BannerRewardAmplitudeEvent(CurrencyAmplitudeAnalytic.Source[sourceType], lootboxId, garantId, garantIdStep, garantChance, amount, currencyType);
		((IAnalyticSender<AmplitudeEvent>)(object)_amplitude).AddEvent((AmplitudeEvent)bannerRewardAmplitudeEvent);
	}
}
