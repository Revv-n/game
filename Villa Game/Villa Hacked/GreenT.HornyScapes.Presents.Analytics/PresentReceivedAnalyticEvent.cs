using GreenT.HornyScapes.Analytics;

namespace GreenT.HornyScapes.Presents.Analytics;

public class PresentReceivedAnalyticEvent : AmplitudeEvent
{
	private const string EventTypeKey = "currency_present_received";

	private const string AmountKey = "amount";

	private const string TypeKey = "type";

	private const string SourceKey = "source";

	public PresentReceivedAnalyticEvent(string presentId, int count, string sourceType)
		: base("currency_present_received")
	{
		((AnalyticsEvent)this).AddEventParams("amount", (object)count);
		((AnalyticsEvent)this).AddEventParams("type", (object)presentId);
		((AnalyticsEvent)this).AddEventParams("source", (object)sourceType);
	}
}
