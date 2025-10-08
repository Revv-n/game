using GreenT.Types;

namespace GreenT.HornyScapes.Analytics;

public sealed class MiniEventCurrencyAmplitudeEvent : AmplitudeEvent
{
	private const string ANALYTIC_SOURCE = "source";

	private const string CURRENCY_ID = "id";

	private const string AMOUNT = "amount";

	private const string CONTENT_TYPE = "content_type";

	public MiniEventCurrencyAmplitudeEvent(string event_name, int diff, string source, int currencID, ContentType contentType)
		: base(event_name)
	{
		diff = ((diff > 0) ? diff : (-diff));
		((AnalyticsEvent)this).AddEventParams("source", (object)source);
		((AnalyticsEvent)this).AddEventParams("id", (object)currencID);
		((AnalyticsEvent)this).AddEventParams("amount", (object)diff);
		((AnalyticsEvent)this).AddEventParams("content_type", (object)contentType.ToString());
	}
}
