using GreenT.HornyScapes.Analytics;
using GreenT.Types;

namespace GreenT.HornyScapes.Sellouts.Analytics;

public sealed class SelloutAnalytic : BaseAnalytic
{
	public SelloutAnalytic(IAmplitudeSender<AmplitudeEvent> amplitude)
		: base(amplitude)
	{
	}

	public void SendPointsReceivedEvent(int points, CurrencyAmplitudeAnalytic.SourceType sourceType, ContentType contentType)
	{
		SelloutPointsEvent analyticsEvent = new SelloutPointsEvent(points, CurrencyAmplitudeAnalytic.Source[sourceType], contentType);
		amplitude.AddEvent(analyticsEvent);
	}

	public void SendRewardReceivedEvent(int selloutId, int rewardsCount)
	{
		SelloutRewardEvent analyticsEvent = new SelloutRewardEvent(selloutId, rewardsCount);
		amplitude.AddEvent(analyticsEvent);
	}
}
