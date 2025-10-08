using System;
using GreenT.HornyScapes.Monetization;
using GreenT.Net;

namespace GreenT.HornyScapes;

public class HaremInvoiceStatusRequester : HTTPGetRequest<Response<PaymentIntentData>>
{
	private const string EndpointTemplate = "https://payment.haremvilla.com/payment/invoices/{0}";

	public IObservable<Response<PaymentIntentData>> GetInvoiceStatus(string invoiceId)
	{
		return GetRequest("https://payment.haremvilla.com/payment/invoices/{0}", invoiceId);
	}
}
