using System;
using System.Text;
using GreenT.HornyScapes.Monetization;
using GreenT.Net;
using Newtonsoft.Json;
using Nutaku.Unity;
using UniRx;
using UnityEngine;

namespace GreenT.Nutaku;

public static class NutakuExtensions
{
	public static T ParseResult<T>(this RawResult rawResult)
	{
		if (rawResult.statusCode > 199 && rawResult.statusCode < 300)
		{
			return RestApi.HandleRequestCallback<T>(rawResult).GetFirstEntry();
		}
		string @string = Encoding.UTF8.GetString(rawResult.body);
		throw new Exception("Nutaku API response status: " + rawResult.statusCode + " content:" + @string);
	}

	public static T ParsePostResult<T>(this RawResult rawResult)
	{
		if (rawResult.statusCode > 199 && rawResult.statusCode < 300)
		{
			MakeRequestResult makeRequestResult = RestApi.HandlePostMakeRequestCallback(rawResult);
			if (makeRequestResult.rc != 200)
			{
				Response response = JsonConvert.DeserializeObject<Response>(makeRequestResult.body);
				throw new Exception($"Request error: [{response.Status}] {response.Error}");
			}
			return JsonConvert.DeserializeObject<T>(makeRequestResult.body);
		}
		string @string = Encoding.UTF8.GetString(rawResult.body);
		throw new Exception("Nutaku API response status: " + rawResult.statusCode + " content:" + @string);
	}

	public static IObservable<RawResult> GetPayment(string paymentID, MonoBehaviour monoBehaviour, PaymentQueryParameterBuilder query = null)
	{
		Subject<RawResult> subject = new Subject<RawResult>();
		try
		{
			RestApiHelper.GetPayment(SdkPlugin.loginInfo.userId, paymentID, monoBehaviour, ProcessResponse);
			return subject;
		}
		catch (Exception ex)
		{
			subject.Dispose();
			throw ex;
		}
		void ProcessResponse(RawResult rawResult)
		{
			subject.OnNext(rawResult);
			subject.OnCompleted();
			subject.Dispose();
		}
	}

	public static PaymentIntentData.PaymentStatus GetPaymentStatus(WebViewEventKind kind)
	{
		return kind switch
		{
			WebViewEventKind.Succeeded => PaymentIntentData.PaymentStatus.succeeded, 
			_ => PaymentIntentData.PaymentStatus.failed, 
		};
	}

	public static PaymentIntentData.PaymentStatus GetPaymentStatus(Payment payment)
	{
		if (payment.status.Value == 2)
		{
			return PaymentIntentData.PaymentStatus.succeeded;
		}
		return PaymentIntentData.PaymentStatus.failed;
	}
}
