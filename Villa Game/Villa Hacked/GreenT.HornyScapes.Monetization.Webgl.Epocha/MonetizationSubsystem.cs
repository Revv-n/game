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

	public IObservable<CheckoutData> OnOpenForm => Observable.AsObservable<CheckoutData>((IObservable<CheckoutData>)onOpenForm);

	public IObservable<PaymentIntentData> OnCompleted => Observable.AsObservable<PaymentIntentData>((IObservable<PaymentIntentData>)onCompleted);

	public IObservable<string> OnFailed => Observable.AsObservable<string>((IObservable<string>)onFailed);

	public MonetizationSubsystem(User userData, IWindowsManager windowsManager, CheckoutRequest checkoutRequest, PaymentIntentRequest paymentIntentRequest, ReceivedRequest receivedRequest, ResponseStatusSystem statusSystem, PlayerStats playerStats)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
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
		checkoutStream = ObservableExtensions.Subscribe<Response<CheckoutData>>(checkoutRequest.Post(user.PlayerID, currentProduct.LotID, currentProduct.ItemId.ToString()), (Action<Response<CheckoutData>>)OnCheckout, (Action<Exception>)delegate(Exception ex)
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
		IConnectableObservable<PaymentIntentData> obj = Observable.Publish<PaymentIntentData>(Observable.Select<Response<PaymentIntentData>, PaymentIntentData>(Observable.Take<Response<PaymentIntentData>>(Observable.SkipWhile<Response<PaymentIntentData>>(Observable.Do<Response<PaymentIntentData>>(Observable.Catch<Response<PaymentIntentData>, Exception>(paymentIntentRequest.GetRequest(checkoutData.id), (Func<Exception, IObservable<Response<PaymentIntentData>>>)delegate(Exception ex)
		{
			throw ex.SendException("incorrect request PostRequestType.PaymentIntent");
		}), (Action<Response<PaymentIntentData>>)delegate
		{
		}), (Func<Response<PaymentIntentData>, bool>)((Response<PaymentIntentData> _req) => _req.Data.Status != "succeeded")), 1), (Func<Response<PaymentIntentData>, PaymentIntentData>)((Response<PaymentIntentData> _req) => _req.Data)));
		IObservable<Response> observable = Observable.Catch<Response, Exception>(Observable.ContinueWith<PaymentIntentData, Response>((IObservable<PaymentIntentData>)obj, (Func<PaymentIntentData, IObservable<Response>>)SendReceivedRequest), (Func<Exception, IObservable<Response>>)delegate(Exception ex)
		{
			throw ex.SendException("ReceivedRequest error");
		});
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<PaymentIntentData>(Observable.CombineLatest<PaymentIntentData, Response, PaymentIntentData>((IObservable<PaymentIntentData>)obj, observable, (Func<PaymentIntentData, Response, PaymentIntentData>)((PaymentIntentData x, Response y) => x)), (Action<PaymentIntentData>)ProcessDataOnComplete, (Action<Exception>)delegate(Exception ex)
		{
			Abort("Payment was corrupt: " + ex.Message);
		}), (ICollection<IDisposable>)checkPaymentStream);
		DisposableExtensions.AddTo<IDisposable>(obj.Connect(), (ICollection<IDisposable>)checkPaymentStream);
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
		return Observable.Where<Response>(receivedRequest.Post(fields, data.ID), (Func<Response, bool>)((Response _response) => _response.Status == 200));
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
