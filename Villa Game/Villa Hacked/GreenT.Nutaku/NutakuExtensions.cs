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
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (rawResult.statusCode > 199 && rawResult.statusCode < 300)
		{
			return RestApi.HandleRequestCallback<T>(rawResult).GetFirstEntry();
		}
		string @string = Encoding.UTF8.GetString(rawResult.body);
		throw new Exception("Nutaku API response status: " + rawResult.statusCode + " content:" + @string);
	}

	public static T ParsePostResult<T>(this RawResult rawResult)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (rawResult.statusCode > 199 && rawResult.statusCode < 300)
		{
			MakeRequestResult val = RestApi.HandlePostMakeRequestCallback(rawResult);
			if (val.rc != 200)
			{
				Response response = JsonConvert.DeserializeObject<Response>(val.body);
				throw new Exception($"Request error: [{response.Status}] {response.Error}");
			}
			return JsonConvert.DeserializeObject<T>(val.body);
		}
		string @string = Encoding.UTF8.GetString(rawResult.body);
		throw new Exception("Nutaku API response status: " + rawResult.statusCode + " content:" + @string);
	}

	public static IObservable<RawResult> GetPayment(string paymentID, MonoBehaviour monoBehaviour, PaymentQueryParameterBuilder query = null)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Expected O, but got Unknown
		Subject<RawResult> subject = new Subject<RawResult>();
		try
		{
			RestApiHelper.GetPayment(SdkPlugin.loginInfo.userId, paymentID, monoBehaviour, new callbackFunctionDelegate(ProcessResponse), (PaymentQueryParameterBuilder)null);
			return (IObservable<RawResult>)subject;
		}
		catch (Exception ex)
		{
			subject.Dispose();
			throw ex;
		}
		void ProcessResponse(RawResult rawResult)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			subject.OnNext(rawResult);
			subject.OnCompleted();
			subject.Dispose();
		}
	}

	public static PaymentIntentData.PaymentStatus GetPaymentStatus(WebViewEventKind kind)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		if ((int)kind != 1)
		{
			if (kind - 2 > 1)
			{
			}
			return PaymentIntentData.PaymentStatus.failed;
		}
		return PaymentIntentData.PaymentStatus.succeeded;
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
