using System;
using System.Collections.Generic;
using GreenT.Net;
using GreenT.UI;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Monetization.Webgl.Epocha;

public class MonetizationSubsystem : IDisposable
{
	private const string SUCCEEDED_PAYMENT_STATUS = "succeeded";

	private Subject<CheckoutData> onOpenForm = new Subject<CheckoutData>();

	private Subject<PaymentIntentData> onCompleted = new Subject<PaymentIntentData>();

	private Subject<string> onFailed = new Subject<string>();

	private EpochaPopup epochaPopup;

	private IDisposable checkoutStream;

	private IDisposable offlinePaymentStream;

	private int currentProductMonetizationId;

	private readonly CompositeDisposable checkPaymentStream = new CompositeDisposable();

	private readonly User user;

	private readonly IWindowsManager windowsManager;

	private readonly ResponseStatusSystem statusSystem;

	private readonly PlayerStats playerStats;

	private readonly CheckoutRequest checkoutRequest;

	private readonly PaymentIntentRequest paymentIntentRequest;

	private readonly ReceivedRequest receivedRequest;

	public IObservable<CheckoutData> OnOpenForm => onOpenForm.AsObservable();

	public IObservable<PaymentIntentData> OnCompleted => onCompleted.AsObservable();

	public IObservable<string> OnFailed => onFailed.AsObservable();

	public MonetizationSubsystem(User userData, IWindowsManager windowsManager, CheckoutRequest checkoutRequest, PaymentIntentRequest paymentIntentRequest, ReceivedRequest receivedRequest, ResponseStatusSystem statusSystem, PlayerStats playerStats)
	{
		user = userData;
		this.windowsManager = windowsManager;
		this.checkoutRequest = checkoutRequest;
		this.paymentIntentRequest = paymentIntentRequest;
		this.receivedRequest = receivedRequest;
		this.statusSystem = statusSystem;
		this.playerStats = playerStats;
	}

	public void OnIssueTheProductBought(PaymentIntentData data)
	{
		onCompleted.OnNext(data);
	}

	public void BuyProduct(Product product)
	{
		currentProductMonetizationId = product.ItemId;
		GetWindows();
		epochaPopup.Open();
		ReactiveProperty<int> checkoutAttemptCount = playerStats.CheckoutAttemptCount;
		int value = checkoutAttemptCount.Value + 1;
		checkoutAttemptCount.Value = value;
		SendCheckout(product);
	}

	private void SendCheckout(Product currentProduct)
	{
		checkoutStream?.Dispose();
		checkoutStream = checkoutRequest.Post(user.PlayerID, currentProduct.LotID, currentProduct.ItemId.ToString()).Subscribe(OnCheckout, delegate(Exception ex)
		{
			throw SendErrorFail(ex, "get error in checkout response from server");
		});
	}

	private void OnCheckout(Response<CheckoutData> request)
	{
		statusSystem.SendLog(request);
		if (string.IsNullOrEmpty(request.Data.url))
		{
			throw SendErrorFail(new Exception("checkout data has empty url"), string.Empty);
		}
		try
		{
			Application.OpenURL(request.Data.url);
		}
		catch (Exception exception)
		{
			throw SendErrorFail(exception, "Cant open url for checkout payment: " + request.Data.url);
		}
		try
		{
			onOpenForm?.OnNext(request.Data);
		}
		catch (Exception exception2)
		{
			throw SendErrorFail(exception2, "Error on 'onOpenForm' emit");
		}
		CheckStatus(request.Data);
	}

	private Exception SendErrorFail(Exception exception, string errMsg)
	{
		Abort(errMsg);
		throw exception.SendException(errMsg);
	}

	private void CheckStatus(CheckoutData checkoutData)
	{
		checkPaymentStream.Clear();
		IConnectableObservable<PaymentIntentData> connectableObservable = (from _req in paymentIntentRequest.GetRequest(checkoutData.id).Catch(delegate(Exception ex)
			{
				throw ex.SendException("incorrect request PostRequestType.PaymentIntent");
			}).Do(delegate
			{
			})
				.SkipWhile((Response<PaymentIntentData> _req) => _req.Data.Status != "succeeded")
				.Take(1)
			select _req.Data).Publish();
		IObservable<Response> right = connectableObservable.ContinueWith(SendReceivedRequest).Catch(delegate(Exception ex)
		{
			throw ex.SendException("ReceivedRequest error");
		});
		connectableObservable.CombineLatest(right, (PaymentIntentData x, Response y) => x).Subscribe(ProcessDataOnComplete, delegate(Exception ex)
		{
			Abort("Payment was corrupt: " + ex.Message);
		}).AddTo(checkPaymentStream);
		connectableObservable.Connect().AddTo(checkPaymentStream);
	}

	private void ProcessDataOnComplete(PaymentIntentData obj)
	{
		if ((bool)epochaPopup && epochaPopup.IsOpened)
		{
			epochaPopup.Close();
		}
		onCompleted.OnNext(obj);
	}

	public IObservable<Response> SendReceivedRequest(PaymentIntentData data)
	{
		IDictionary<string, string> fields = new Dictionary<string, string>();
		return from _response in receivedRequest.Post(fields, data.ID)
			where _response.Status == 200
			select _response;
	}

	public void AbortPayment()
	{
		Abort("abort");
	}

	private void Abort(string error)
	{
		checkPaymentStream.Clear();
		checkoutStream?.Dispose();
		onFailed.OnNext($"Error message: {error}. Monetization id: {currentProductMonetizationId}");
	}

	public void Clear()
	{
		checkoutStream?.Dispose();
		checkPaymentStream.Clear();
	}

	private void GetWindows()
	{
		if (!epochaPopup)
		{
			epochaPopup = windowsManager.Get<EpochaPopup>();
		}
	}

	public void Dispose()
	{
		Clear();
		checkPaymentStream.Dispose();
		onCompleted?.OnCompleted();
		onCompleted?.Dispose();
		onFailed?.OnCompleted();
		onFailed?.Dispose();
	}
}
