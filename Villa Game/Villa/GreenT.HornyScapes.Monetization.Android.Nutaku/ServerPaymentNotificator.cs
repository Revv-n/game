using System;
using System.Collections.Generic;
using GreenT.Net;
using GreenT.Nutaku;
using GreenT.Settings.Data;
using Nutaku.Unity;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Monetization.Android.Nutaku;

public sealed class ServerPaymentNotificator
{
	private readonly string setPaymentStatusUrl;

	private readonly ReceivedRequest setRecievedRequest;

	private readonly MonoBehaviour monoBehavior;

	public ServerPaymentNotificator(IRequestUrlResolver urlResolver, MonoBehaviour monoBehavior, ReceivedRequest receivedRequest)
	{
		setPaymentStatusUrl = urlResolver.PostRequestUrl(PostRequestType.PaymentStatusUpdate);
		setRecievedRequest = receivedRequest;
		this.monoBehavior = monoBehavior;
	}

	public IObservable<Response<PaymentIntentData>> UpdatePaymentStatus(WebViewEvent result)
	{
		PaymentIntentData.PaymentStatus paymentStatus = NutakuExtensions.GetPaymentStatus(result.kind);
		return UpdatePaymentStatusAcrossNutakuApi(result.value, paymentStatus);
	}

	public IObservable<Response<PaymentIntentData>> UpdatePaymentStatus(Payment payment)
	{
		PaymentIntentData.PaymentStatus paymentStatus = NutakuExtensions.GetPaymentStatus(payment);
		return UpdatePaymentStatusAcrossNutakuApi(payment.paymentId, paymentStatus);
	}

	public IObservable<Response<PaymentIntentData>> UpdatePaymentStatusAcrossNutakuApi(string paymentID, PaymentIntentData.PaymentStatus status)
	{
		string callbackUrl = string.Format(setPaymentStatusUrl, paymentID, status);
		Dictionary<string, string> postData = new Dictionary<string, string>();
		Subject<Response<PaymentIntentData>> responseSubject = new Subject<Response<PaymentIntentData>>();
		RestApiHelper.PostMakeRequest(callbackUrl, postData, monoBehavior, ProcessResponse);
		return responseSubject;
		void ProcessResponse(RawResult rawResult)
		{
			try
			{
				Response<PaymentIntentData> value = rawResult.ParsePostResult<Response<PaymentIntentData>>();
				responseSubject.OnNext(value);
			}
			catch (Exception innerException)
			{
				Exception ex = new Exception("Error on post request to \"" + callbackUrl + "\"", innerException);
				responseSubject.OnError(ex);
				throw ex;
			}
			finally
			{
				responseSubject.OnCompleted();
				responseSubject.Dispose();
			}
		}
	}

	public IObservable<Response> SetRecieved(string invoiceID)
	{
		return setRecievedRequest.PostWithEmptyFields(invoiceID);
	}
}
