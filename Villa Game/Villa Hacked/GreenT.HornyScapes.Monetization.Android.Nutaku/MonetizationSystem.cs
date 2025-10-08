using System;
using System.Collections.Generic;
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

	public IObservable<Unit> OnSuccess => Observable.Select<Transaction, Unit>(OnSucceeded, (Func<Transaction, Unit>)((Transaction _) => default(Unit)));

	public MonetizationSystem(StartPaymentRequest startPaymentRequest, ServerPaymentNotificator serverNotificator, MonoBehaviour monoBehaviour)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
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
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected O, but got Unknown
		try
		{
			startPaymentRequest.Request(convertedPrice, monetizationID, itemName, itemDescription, itemImageUrl, new callbackFunctionDelegate(RedirectToStore));
		}
		catch (Exception)
		{
			HandleFail("Unhandled exception on payment");
		}
	}

	private void RedirectToStore(RawResult rawResult)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		try
		{
			Payment payment = rawResult.ParseResult<Payment>();
			DebugResponsePayment(payment);
			SdkPlugin.OpenPaymentView(payment, (PaymentResultDelegate)delegate(WebViewEvent _result)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		DebugPaymentResult(result);
		if (ValidatePayment(result))
		{
			RestApiHelper.GetPayment(SdkPlugin.loginInfo.userId, payment.paymentId, monoBehaviour, new callbackFunctionDelegate(UpdatePaymentState), (PaymentQueryParameterBuilder)null);
			return;
		}
		Debug.Log("WebViewKind ID: " + result.value + " Payment ID: " + payment.paymentId);
		AbortPaymentNotifyServer(result.kind, payment.paymentId);
	}

	private bool ValidatePayment(WebViewEvent result)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		return (int)result.kind == 1;
	}

	private void AbortPaymentNotifyServer(WebViewEventKind kind, string paymentId)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		PaymentIntentData.PaymentStatus paymentStatus = NutakuExtensions.GetPaymentStatus(kind);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Response<PaymentIntentData>>(serverNotificator.UpdatePaymentStatusAcrossNutakuApi(paymentId, paymentStatus), (Action<Response<PaymentIntentData>>)delegate
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			ProcessPaymentAbort(kind);
		}, (Action<Exception>)delegate(Exception ex)
		{
			HandleFail(ex.ToString());
		}), (ICollection<IDisposable>)notificationStream);
	}

	private void ProcessPaymentAbort(WebViewEventKind kind)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Invalid comparison between Unknown and I4
		string message = (((int)kind == 2) ? "Error during purchase" : (((int)kind != 3) ? ("WebViewEventKind type: " + ((object)(WebViewEventKind)(ref kind)).ToString()) : "User cancelled the purchase"));
		HandleFail(message);
	}

	private void UpdatePaymentState(RawResult rawResult)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
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
		IConnectableObservable<Response<PaymentIntentData>> obj = Observable.Publish<Response<PaymentIntentData>>(serverNotificator.UpdatePaymentStatus(payment));
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Transaction>(Observable.Select<Response<PaymentIntentData>, Transaction>(Observable.Where<Response<PaymentIntentData>>(Observable.Where<Response<PaymentIntentData>>((IObservable<Response<PaymentIntentData>>)obj, (Func<Response<PaymentIntentData>, bool>)IsRequestSuccessful), (Func<Response<PaymentIntentData>, bool>)((Response<PaymentIntentData> _response) => _response.Data.Status == PaymentIntentData.PaymentStatus.succeeded.ToString())), (Func<Response<PaymentIntentData>, Transaction>)((Response<PaymentIntentData> _response) => new Transaction(_response.Data.ItemID, isValidated: false, _response.Data.ID))), (Action<Transaction>)NotifyAboutReceivedTransaction, (Action<Exception>)delegate(Exception ex)
		{
			HandleFail(ex.ToString());
		}), (ICollection<IDisposable>)notificationStream);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Response<PaymentIntentData>>(Observable.Where<Response<PaymentIntentData>>((IObservable<Response<PaymentIntentData>>)obj, (Func<Response<PaymentIntentData>, bool>)((Response<PaymentIntentData> x) => !IsRequestSuccessful(x))), (Action<Response<PaymentIntentData>>)delegate(Response<PaymentIntentData> response)
		{
			HandleFail($"Server notification of sucessful payment failed. [{response.Status}] {response.Error}.\nPayment id: " + payment.paymentId);
		}, (Action<Exception>)delegate(Exception ex)
		{
			Debug.LogError(ex.ToString());
		}), (ICollection<IDisposable>)notificationStream);
		DisposableExtensions.AddTo<IDisposable>(obj.Connect(), (ICollection<IDisposable>)notificationStream);
	}

	private void NotifyAboutReceivedTransaction(Transaction transaction)
	{
		IConnectableObservable<Response> obj = Observable.Publish<Response>(serverNotificator.SetRecieved(transaction.Id));
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Response>(Observable.Where<Response>((IObservable<Response>)obj, (Func<Response, bool>)IsRequestSuccessful), (Action<Response>)delegate
		{
			ProcessNotificationSuccess();
		}), (ICollection<IDisposable>)notificationStream);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Response>(Observable.Where<Response>((IObservable<Response>)obj, (Func<Response, bool>)((Response x) => !IsRequestSuccessful(x))), (Action<Response>)delegate(Response response)
		{
			ProcessNotificationFail(response, transaction);
		}, (Action<Exception>)delegate(Exception ex)
		{
			HandleFail(ex.ToString());
		}), (ICollection<IDisposable>)notificationStream);
		DisposableExtensions.AddTo<IDisposable>(obj.Connect(), (ICollection<IDisposable>)notificationStream);
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
