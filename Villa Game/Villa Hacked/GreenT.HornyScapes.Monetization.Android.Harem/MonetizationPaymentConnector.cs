using System;
using GreenT.Data;
using GreenT.HornyScapes.Monetization.Harem;
using GreenT.Net;
using GreenT.UI;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Monetization.Android.Harem;

public class MonetizationPaymentConnector : MonoBehaviour, IPaymentConnector
{
	[Serializable]
	public class PaymentResponse : Response
	{
		public string url;

		public string payment_id;

		public PaymentResponse(int status, string error = "")
			: base(status, error)
		{
		}
	}

	private Action<int> _success;

	private Action<int> _failure;

	private int _monetizationId;

	private WebViewHarem _webView;

	private ScreenOrientation _screenOrientation;

	private IDisposable _watchPaymentSubscription;

	private IDisposable _webViewSubscription;

	private IDisposable _paymentSelectedSubscription;

	private IDisposable _paymentRequestSubscription;

	private IDisposable _exitClickedSubscription;

	private IDisposable _getImageSubscription;

	private readonly Subject<Unit> _cancelWatchPayment = new Subject<Unit>();

	private float _priceForBitcoin = 19.95f;

	private string _paymentId;

	private bool _suppressWebViewCloseCallback;

	[Inject]
	private IDataStorage _dataStorage;

	[Inject]
	private IWindowsManager _windowsManager;

	[Inject]
	private HaremPaymentRequester _requester;

	[Inject]
	private HaremInvoiceStatusRequester _invoiceStatusRequester;

	[Inject]
	private SignalBus _signalBus;

	[Inject]
	private void Construct()
	{
		_signalBus.Subscribe<CancelPaymentSignal>((Action)OnPaymentCancelled);
	}

	public void Connect(int monetizationID, string itemName, string price, string description, string imageUrl, Action<int> success, Action<int> failure)
	{
		_success = success;
		_failure = failure;
		_monetizationId = monetizationID;
		_screenOrientation = Screen.orientation;
		ShowChoosePaymentType(monetizationID, itemName, price, imageUrl);
	}

	public void OnPaymentSuccess(int monetizationId)
	{
		_success(monetizationId);
	}

	public void OnPaymentFailed(int monetizationId)
	{
		_failure(monetizationId);
	}

	private void ShowChoosePaymentType(int monetizationID, string itemName, string price, string imageUrl)
	{
		_getImageSubscription?.Dispose();
		_getImageSubscription = ObservableExtensions.Subscribe<Sprite>(HttpRequestExecutor.GetSprite(imageUrl), (Action<Sprite>)delegate(Sprite sprite)
		{
			ShowPopup(monetizationID, itemName, price, sprite);
		}, (Action<Exception>)delegate
		{
			ShowPopup(monetizationID, itemName, price, null);
		});
	}

	private void ShowPopup(int monetizationID, string itemName, string price, Sprite imageUrl)
	{
		_paymentSelectedSubscription?.Dispose();
		_exitClickedSubscription?.Dispose();
		bool isCryptoAvailable = false;
		float.TryParse(price, out var result);
		if (result >= _priceForBitcoin)
		{
			isCryptoAvailable = true;
		}
		HaremChoosePaymentPopup typePyamentPopup = _windowsManager.Get<HaremChoosePaymentPopup>();
		typePyamentPopup.Open();
		typePyamentPopup.ShowItem(itemName, price, imageUrl, isCryptoAvailable);
		_paymentSelectedSubscription = ObservableExtensions.Subscribe<PaymentMethod>(typePyamentPopup.OnPaymentSelected, (Action<PaymentMethod>)delegate(PaymentMethod method)
		{
			switch (method)
			{
			case PaymentMethod.Centrobill:
				StartPurchase(monetizationID, itemName, price, "centrobill");
				typePyamentPopup.Close();
				break;
			case PaymentMethod.Bitcoin:
				StartPurchase(monetizationID, itemName, price, "centrobill/crypto");
				typePyamentPopup.Close();
				break;
			}
		});
		_exitClickedSubscription = ObservableExtensions.Subscribe<Unit>(typePyamentPopup.OnExitClicked, (Action<Unit>)delegate
		{
			OnPaymentFailed(_monetizationId);
			typePyamentPopup.Close();
		});
	}

	private void StartPurchase(int monetizationID, string itemName, string price, string typePayment)
	{
		_paymentRequestSubscription?.Dispose();
		_paymentRequestSubscription = ObservableExtensions.Subscribe<PaymentResponse>(_requester.GetPaymentPage(typePayment, _dataStorage.GetString("player_id"), monetizationID), (Action<PaymentResponse>)delegate(PaymentResponse response)
		{
			OpenWebView(response);
		}, (Action<Exception>)delegate
		{
			OnPaymentFailed(monetizationID);
		});
	}

	private void OpenWebView(PaymentResponse response)
	{
		_webViewSubscription?.Dispose();
		_suppressWebViewCloseCallback = false;
		_webView = new WebViewHarem();
		_webViewSubscription = ObservableExtensions.Subscribe<Unit>(_webView.OnCloseWebViewObservable, (Action<Unit>)delegate
		{
			if (!_suppressWebViewCloseCallback)
			{
				WebViewClosed();
			}
		});
		_paymentId = response.payment_id;
		_webView.OpenWebView(response.url);
	}

	private void WebViewClosed()
	{
		_paymentRequestSubscription?.Dispose();
		_watchPaymentSubscription?.Dispose();
		int retryCount = 0;
		_watchPaymentSubscription = ObservableExtensions.Subscribe<Response<PaymentIntentData>>(Observable.SelectMany<long, Response<PaymentIntentData>>(Observable.StartWith<long>(Observable.Interval(TimeSpan.FromSeconds(5.0)), 0L), (Func<long, IObservable<Response<PaymentIntentData>>>)((long _) => _invoiceStatusRequester.GetInvoiceStatus(_paymentId))), (Action<Response<PaymentIntentData>>)delegate(Response<PaymentIntentData> response)
		{
			if (string.Compare(response.Data.Status, "success", ignoreCase: true) == 0)
			{
				SuccesPayment(response.Data.InvoiceID);
				_watchPaymentSubscription?.Dispose();
			}
			else
			{
				retryCount++;
				if (retryCount > 100)
				{
					_watchPaymentSubscription?.Dispose();
					OnPaymentFailed(_monetizationId);
				}
			}
		});
	}

	private void SuccesPayment(string invoiceID)
	{
		_watchPaymentSubscription?.Dispose();
		if (!string.IsNullOrEmpty(invoiceID))
		{
			_dataStorage.SetString("lastInvoiceID", invoiceID);
		}
		OnPaymentSuccess(_monetizationId);
	}

	private void OnPaymentCancelled()
	{
		_watchPaymentSubscription?.Dispose();
		OnPaymentFailed(_monetizationId);
		_suppressWebViewCloseCallback = true;
		_webViewSubscription?.Dispose();
		if (_webView != null)
		{
			_webView.CloseWebView();
		}
	}

	private void OnDestroy()
	{
		_webViewSubscription?.Dispose();
	}
}
