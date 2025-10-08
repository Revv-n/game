using System;
using GreenT.Net;
using GreenT.Nutaku;
using Nutaku.Unity;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Monetization.Android.Nutaku;

public class MonetizationSystem : MonetizationSystem<Transaction>, IMonetizationAdapter
{
	private readonly StartPaymentRequest startPaymentRequest;

	private readonly ServerPaymentNotificator serverNotificator;

	private int currentProductMonetizationId;

	private readonly MonoBehaviour monoBehaviour;

	private CompositeDisposable notificationStream = new CompositeDisposable();

	public IObservable<Unit> OnSuccess => OnSucceeded.Select((Transaction _) => default(Unit));

	public MonetizationSystem(StartPaymentRequest startPaymentRequest, ServerPaymentNotificator serverNotificator, MonoBehaviour monoBehaviour)
	{
		this.startPaymentRequest = startPaymentRequest;
		this.serverNotificator = serverNotificator;
		this.monoBehaviour = monoBehaviour;
	}

	public void BuyProduct(string lotId, int monetizationID, int entityId, string price = null, string region = null, string itemName = null, string itemDescription = null, string itemImageUrl = null, string currenyType = null)
	{
		currentProductMonetizationId = monetizationID;
		if (!int.TryParse(price, out var result))
		{
			string message = $"itemId = {entityId} has incorrect price = {price}";
			HandleFail(message);
		}
		else
		{
			BuyProduct(lotId, monetizationID, price);
			CreatePaymentOnServer(monetizationID, itemName, itemDescription, itemImageUrl, result);
		}
	}

	private void CreatePaymentOnServer(int monetizationID, string itemName, string itemDescription, string itemImageUrl, int convertedPrice)
	{
		try
		{
			startPaymentRequest.Request(convertedPrice, monetizationID, itemName, itemDescription, itemImageUrl, RedirectToStore);
		}
		catch (Exception)
		{
			HandleFail("Unhandled exception on payment");
		}
	}

	private void RedirectToStore(RawResult rawResult)
	{
		try
		{
			Payment payment = rawResult.ParseResult<Payment>();
			DebugResponsePayment(payment);
			SdkPlugin.OpenPaymentView(payment, delegate(WebViewEvent _result)
			{
				ProcessResponseFromStore(_result, payment);
			});
		}
		catch (Exception ex)
		{
			HandleFail(ex.Message);
		}
	}

	private void ProcessResponseFromStore(WebViewEvent result, Payment payment)
	{
		DebugPaymentResult(result);
		if (ValidatePayment(result))
		{
			RestApiHelper.GetPayment(SdkPlugin.loginInfo.userId, payment.paymentId, monoBehaviour, UpdatePaymentState);
			return;
		}
		Debug.Log("WebViewKind ID: " + result.value + " Payment ID: " + payment.paymentId);
		AbortPaymentNotifyServer(result.kind, payment.paymentId);
	}

	private bool ValidatePayment(WebViewEvent result)
	{
		return result.kind == WebViewEventKind.Succeeded;
	}

	private void AbortPaymentNotifyServer(WebViewEventKind kind, string paymentId)
	{
		PaymentIntentData.PaymentStatus paymentStatus = NutakuExtensions.GetPaymentStatus(kind);
		serverNotificator.UpdatePaymentStatusAcrossNutakuApi(paymentId, paymentStatus).Subscribe(delegate
		{
			ProcessPaymentAbort(kind);
		}, delegate(Exception ex)
		{
			HandleFail(ex.ToString());
		}).AddTo(notificationStream);
	}

	private void ProcessPaymentAbort(WebViewEventKind kind)
	{
		HandleFail(kind switch
		{
			WebViewEventKind.Failed => "Error during purchase", 
			WebViewEventKind.Cancelled => "User cancelled the purchase", 
			_ => "WebViewEventKind type: " + kind, 
		});
	}

	private void UpdatePaymentState(RawResult rawResult)
	{
		Payment payment;
		try
		{
			payment = rawResult.ParseResult<Payment>();
		}
		catch (Exception ex2)
		{
			HandleFail(ex2.Message);
			return;
		}
		DebugResponsePayment(payment);
		IConnectableObservable<Response<PaymentIntentData>> connectableObservable = serverNotificator.UpdatePaymentStatus(payment).Publish();
		(from _response in connectableObservable.Where(IsRequestSuccessful)
			where _response.Data.Status == PaymentIntentData.PaymentStatus.succeeded.ToString()
			select new Transaction(_response.Data.ItemID, isValidated: false, _response.Data.ID)).Subscribe(NotifyAboutReceivedTransaction, delegate(Exception ex)
		{
			HandleFail(ex.ToString());
		}).AddTo(notificationStream);
		connectableObservable.Where((Response<PaymentIntentData> x) => !IsRequestSuccessful(x)).Subscribe(delegate(Response<PaymentIntentData> response)
		{
			HandleFail($"Server notification of sucessful payment failed. [{response.Status}] {response.Error}.\nPayment id: " + payment.paymentId);
		}, delegate(Exception ex)
		{
			Debug.LogError(ex.ToString());
		}).AddTo(notificationStream);
		connectableObservable.Connect().AddTo(notificationStream);
	}

	private void NotifyAboutReceivedTransaction(Transaction transaction)
	{
		IConnectableObservable<Response> connectableObservable = serverNotificator.SetRecieved(transaction.Id).Publish();
		connectableObservable.Where(IsRequestSuccessful).Subscribe(delegate
		{
			ProcessNotificationSuccess();
		}).AddTo(notificationStream);
		connectableObservable.Where((Response x) => !IsRequestSuccessful(x)).Subscribe(delegate(Response response)
		{
			ProcessNotificationFail(response, transaction);
		}, delegate(Exception ex)
		{
			HandleFail(ex.ToString());
		}).AddTo(notificationStream);
		connectableObservable.Connect().AddTo(notificationStream);
		void ProcessNotificationFail(Response response, Transaction transaction)
		{
			HandleFail($"Serser response status {response.Status}. " + $" Server response Status code: {transaction.StatusCode}");
		}
		void ProcessNotificationSuccess()
		{
			transaction.IsValidated = true;
			onSucceeded.OnNext(transaction);
		}
	}

	private void HandleFail(string message)
	{
		onFailed.OnNext($"Error message: {message}. Monetizationd id: {currentProductMonetizationId}");
	}

	private bool IsRequestSuccessful(Response response)
	{
		if (response.Status != 0)
		{
			return response.Status == 200;
		}
		return true;
	}

	private void DebugResponsePayment(Payment responsePayment)
	{
	}

	private void DebugErrorResponsePayment(RawResult rawResult)
	{
	}

	private void DebugPaymentResult(WebViewEvent result)
	{
	}
}
