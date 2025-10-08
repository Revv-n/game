using System;
using GreenT.Net;
using UniRx;

namespace GreenT.HornyScapes.Monetization;

public class PaymentIntentRequest : HTTPGetRequest<Response<PaymentIntentData>>
{
	private const float CHECK_STRIPE_DELAY = 5f;

	private readonly string requestUrl;

	public PaymentIntentRequest(string url)
	{
		requestUrl = url;
	}

	public IObservable<Response<PaymentIntentData>> GetRequest(params string[] args)
	{
		return GetRequest(requestUrl, args).Delay(TimeSpan.FromSeconds(5.0)).Repeat();
	}
}
