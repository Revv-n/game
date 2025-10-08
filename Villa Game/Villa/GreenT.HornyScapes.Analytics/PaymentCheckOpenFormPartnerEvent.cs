namespace GreenT.HornyScapes.Analytics;

public class PaymentCheckOpenFormPartnerEvent : PartnerEvent
{
	private const string EVENT_NAME_KEY = "open_form";

	public PaymentCheckOpenFormPartnerEvent(object invoiceId, object url)
		: base("open_form")
	{
		AddEventParams("invoice_id", invoiceId.ToString());
		AddEventParams("url", url.ToString());
	}
}
