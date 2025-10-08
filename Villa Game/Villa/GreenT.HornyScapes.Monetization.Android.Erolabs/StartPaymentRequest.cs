using System;
using Nutaku.Unity;
using UnityEngine;

namespace GreenT.HornyScapes.Monetization.Android.Erolabs;

public class StartPaymentRequest
{
	private readonly MonoBehaviour monoBehaviour;

	private readonly PaymentFactory paymentFactory;

	public StartPaymentRequest(PaymentFactory paymentFactory, MonoBehaviour monoBehaviour)
	{
		this.paymentFactory = paymentFactory;
		this.monoBehaviour = monoBehaviour;
	}

	public void Request(int price, int monetizationID, string itemName, string itemDescription, string itemImageUrl, UnityWebRequestUtil.callbackFunctionDelegate callback)
	{
		Payment payment = paymentFactory.CreatePayment(monetizationID, itemName, itemDescription, itemImageUrl, price);
		try
		{
			RestApiHelper.PostPayment(payment, monoBehaviour, callback);
		}
		catch (Exception innerException)
		{
			throw innerException.SendException("Send request to nutaku sdk RestApiHelper.PostPayment(...)");
		}
	}
}
