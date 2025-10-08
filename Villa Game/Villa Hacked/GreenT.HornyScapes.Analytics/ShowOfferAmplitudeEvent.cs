using GreenT.HornyScapes.Bank;

namespace GreenT.HornyScapes.Analytics;

public class ShowOfferAmplitudeEvent : AmplitudeEvent
{
	private const string EVENT_NAME_KEY = "offer_showed";

	private const string ID_KEY = "id";

	private const string ID_PROPERTY_KEY = "show_source";

	public ShowOfferAmplitudeEvent(OfferSettings entity, string showSource)
		: base("offer_showed")
	{
		((AnalyticsEvent)this).AddEventParams("id", (object)entity.ID);
		((AnalyticsEvent)this).AddEventParams("show_source", (object)showSource);
	}
}
