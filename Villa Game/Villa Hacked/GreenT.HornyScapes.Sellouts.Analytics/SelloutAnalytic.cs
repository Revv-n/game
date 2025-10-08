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
		SelloutPointsEvent selloutPointsEvent = new SelloutPointsEvent(points, CurrencyAmplitudeAnalytic.Source[sourceType], contentType);
		((IAnalyticSender<AmplitudeEvent>)(object)amplitude).AddEvent((AmplitudeEvent)selloutPointsEvent);
	}

	public void SendRewardReceivedEvent(int selloutId, int rewardsCount)
	{
		SelloutRewardEvent selloutRewardEvent = new SelloutRewardEvent(selloutId, rewardsCount);
		((IAnalyticSender<AmplitudeEvent>)(object)amplitude).AddEvent((AmplitudeEvent)selloutRewardEvent);
	}
}
