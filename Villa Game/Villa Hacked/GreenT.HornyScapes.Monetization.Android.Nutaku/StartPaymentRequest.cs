using System;
using Nutaku.Unity;
using UnityEngine;

namespace GreenT.HornyScapes.Monetization.Android.Nutaku;

public class StartPaymentRequest
{
	private readonly MonoBehaviour monoBehaviour;

	private readonly PaymentFactory paymentFactory;

	public StartPaymentRequest(PaymentFactory paymentFactory, MonoBehaviour monoBehaviour)
	{
		this.paymentFactory = paymentFactory;
		this.monoBehaviour = monoBehaviour;
	}

	public void Request(int price, int monetizationID, string itemName, string itemDescription, string itemImageUrl, callbackFunctionDelegate callback)
	{
		Payment val = paymentFactory.CreatePayment(monetizationID, itemName, itemDescription, itemImageUrl, price);
		try
		{
			RestApiHelper.PostPayment(val, monoBehaviour, callback, (PaymentQueryParameterBuilder)null);
		}
		catch (Exception innerException)
		{
			throw innerException.SendException("Send request to nutaku sdk RestApiHelper.PostPayment(...)");
		}
	}
}
