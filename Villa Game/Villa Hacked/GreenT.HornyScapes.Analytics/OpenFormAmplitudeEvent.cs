using GreenT.HornyScapes.Monetization;

namespace GreenT.HornyScapes.Analytics;

public class OpenFormAmplitudeEvent : AmplitudeEvent
{
	private const string EVENT_NAME_KEY = "open_form";

	private const string INVOICE_ID = "invoice_id";

	private const string URL = "url";

	public OpenFormAmplitudeEvent(CheckoutData checkoutData)
		: base("open_form")
	{
		((AnalyticsEvent)this).AddEventParams("invoice_id", (object)checkoutData.id);
		((AnalyticsEvent)this).AddEventParams("url", (object)checkoutData.url);
	}
}
