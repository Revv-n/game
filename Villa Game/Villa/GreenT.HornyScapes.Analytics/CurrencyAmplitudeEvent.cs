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
		AddEventParams("source", source);
		if (compositeIdentificator != default(CompositeIdentificator))
		{
			AddEventParams("source_id", compositeIdentificator.ToStringNoFrames());
		}
		diff = ((diff > 0) ? diff : (-diff));
		AddEventParams(type, diff);
		AddEventParams("content_type", contentType);
	}
}
