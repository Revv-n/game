using GreenT.Types;

namespace GreenT.HornyScapes.Analytics;

public sealed class CurrencyAmplitudeEvent : AmplitudeEvent
{
	private const string ANALYTIC_SOURCE = "source";

	private const string SOURCE_ID = "source_id";

	private const string CONTENT_TYPE = "content_type";

	public CurrencyAmplitudeEvent(string event_name, string type, int diff, string source, ContentType contentType, CompositeIdentificator compositeIdentificator = default(CompositeIdentificator))
		: base(event_name)
	{
		((AnalyticsEvent)this).AddEventParams("source", (object)source);
		if (compositeIdentificator != default(CompositeIdentificator))
		{
			((AnalyticsEvent)this).AddEventParams("source_id", (object)compositeIdentificator.ToStringNoFrames());
		}
		diff = ((diff > 0) ? diff : (-diff));
		((AnalyticsEvent)this).AddEventParams(type, (object)diff);
		((AnalyticsEvent)this).AddEventParams("content_type", (object)contentType);
	}
}
