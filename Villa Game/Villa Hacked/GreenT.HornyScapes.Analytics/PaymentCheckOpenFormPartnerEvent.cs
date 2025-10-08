namespace GreenT.HornyScapes.Analytics;

public class PaymentCheckOpenFormPartnerEvent : PartnerEvent
{
	private const string EVENT_NAME_KEY = "open_form";

	public PaymentCheckOpenFormPartnerEvent(object invoiceId, object url)
		: base("open_form")
	{
		((AnalyticsEvent)this).AddEventParams("invoice_id", (object)invoiceId.ToString());
		((AnalyticsEvent)this).AddEventParams("url", (object)url.ToString());
	}
}
