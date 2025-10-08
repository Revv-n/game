using System;
using GreenT.Net;

namespace GreenT.HornyScapes.Monetization.Android.Harem;

public class HaremPaymentRequester : HTTPGetRequest<MonetizationPaymentConnector.PaymentResponse>
{
	private const string EndpointTemplate = "https://payment.haremvilla.com/payment/init/{0}?player_id={1}&monetization_id={2}";

	public IObservable<MonetizationPaymentConnector.PaymentResponse> GetPaymentPage(string paymentType, string playerId, int monetizationId)
	{
		return GetRequest("https://payment.haremvilla.com/payment/init/{0}?player_id={1}&monetization_id={2}", paymentType, playerId, monetizationId);
	}
}
