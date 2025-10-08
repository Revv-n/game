using System;
using GreenT.Net;
using GreenT.Settings.Data;
using UniRx;

namespace GreenT.HornyScapes.Monetization;

public class PaymentStatusUpdateRequest
{
	private readonly IRequestUrlResolver urlResolver;

	public PaymentStatusUpdateRequest(IRequestUrlResolver urlResolver)
	{
		this.urlResolver = urlResolver;
	}

	public IObservable<Response> Get(string paymentID, string status)
	{
		return Observable.ContinueWith<string, Response>(Observable.Start<string>((Func<string>)(() => string.Format(urlResolver.PostRequestUrl(PostRequestType.PaymentStatusUpdate), paymentID, status.ToString()))), (Func<string, IObservable<Response>>)((string _url) => HttpRequestExecutor.GetRequest<Response>(_url)));
	}

	public IObservable<Response> Get(string paymentID, PaymentIntentData.PaymentStatus status)
	{
		return Get(paymentID, status.ToString());
	}
}
