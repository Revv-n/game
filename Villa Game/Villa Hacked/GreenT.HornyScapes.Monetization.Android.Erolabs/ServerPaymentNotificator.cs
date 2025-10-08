using System;
using System.Collections.Generic;
using GreenT.Net;
using GreenT.Nutaku;
using GreenT.Settings.Data;
using Nutaku.Unity;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Monetization.Android.Erolabs;

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
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected O, but got Unknown
		string callbackUrl = string.Format(setPaymentStatusUrl, paymentID, status);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		Subject<Response<PaymentIntentData>> responseSubject = new Subject<Response<PaymentIntentData>>();
		RestApiHelper.PostMakeRequest(callbackUrl, (IEnumerable<KeyValuePair<string, string>>)dictionary, monoBehavior, new callbackFunctionDelegate(ProcessResponse));
		return (IObservable<Response<PaymentIntentData>>)responseSubject;
		void ProcessResponse(RawResult rawResult)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				Response<PaymentIntentData> response = rawResult.ParsePostResult<Response<PaymentIntentData>>();
				responseSubject.OnNext(response);
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

	public IObservable<string> SetRecievedNoResponse(string invoiceID)
	{
		IDictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["transaction_id"] = invoiceID;
		return setRecievedRequest.PostNoResponse(dictionary, invoiceID);
	}
}
