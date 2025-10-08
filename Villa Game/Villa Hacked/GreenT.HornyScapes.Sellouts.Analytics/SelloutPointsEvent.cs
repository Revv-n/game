using GreenT.HornyScapes.Analytics;
using GreenT.Types;

namespace GreenT.HornyScapes.Sellouts.Analytics;

public class SelloutPointsEvent : AmplitudeEvent
{
	private const string EventTypeKey = "currency_sellout_points_received";

	private const string PointsKey = "points";

	private const string SourceKey = "source";

	private const string ContentTypeKey = "content_type";

	public SelloutPointsEvent(int points, string sourceType, ContentType contentType)
		: base("currency_sellout_points_received")
	{
		((AnalyticsEvent)this).AddEventParams("points", (object)points);
		((AnalyticsEvent)this).AddEventParams("points", (object)points);
		((AnalyticsEvent)this).AddEventParams("source", (object)sourceType);
		((AnalyticsEvent)this).AddEventParams("content_type", (object)contentType);
	}
}
