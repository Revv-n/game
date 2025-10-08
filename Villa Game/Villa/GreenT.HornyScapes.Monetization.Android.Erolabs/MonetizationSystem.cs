using System;
using System.Text.RegularExpressions;
using Erolabs.Sdk.Unity;
using UniRx;

namespace GreenT.HornyScapes.Monetization.Android.Erolabs;

public class MonetizationSystem : MonetizationSystem<Transaction>, IMonetizationAdapter
{
	private int currentProductMonetizationId;

	private ErolabsCheckoutRequest checkoutRequest;

	private ErolabsInvoicesFilteredRequest invoicesFilteredRequest;

	private Hyena hyena;

	private ServerPaymentNotificator serverPaymentNotificator;

	private ErolabsGetBalanceRequest erolabsGetBalanceRequest;

	private User user;

	private CompositeDisposable compositeDisposable = new CompositeDisposable();

	private CompositeDisposable notificationDisposable = new CompositeDisposable();

	protected Subject<Tuple<bool, int, Action<Unit>, bool>> onStartBuyButton = new Subject<Tuple<bool, int, Action<Unit>, bool>>();

	protected Subject<Unit> onNotEnoughMoney = new Subject<Unit>();

	public IObservable<Unit> OnSuccess => OnSucceeded.Select((Transaction _) => default(Unit));

	public virtual IObservable<Tuple<bool, int, Action<Unit>, bool>> OnStartBuyButton => onStartBuyButton.AsObservable();

	public virtual IObservable<Unit> OnNotEnoughMoney => onNotEnoughMoney.AsObservable();

	public MonetizationSystem(ErolabsCheckoutRequest checkoutRequest, ErolabsInvoicesFilteredRequest invoicesFilteredRequest, Hyena hyena, ServerPaymentNotificator serverPaymentNotificator, ErolabsGetBalanceRequest erolabsGetBalanceRequest, User user)
	{
		this.checkoutRequest = checkoutRequest;
		this.invoicesFilteredRequest = invoicesFilteredRequest;
		this.hyena = hyena;
		this.serverPaymentNotificator = serverPaymentNotificator;
		this.erolabsGetBalanceRequest = erolabsGetBalanceRequest;
		this.user = user;
	}

	public void BuyProduct(string lotId, int monetizationID, int entityId, string price = null, string region = null, string itemName = null, string itemDescription = null, string itemImageUrl = null, string currenyType = null)
	{
		try
		{
			BuyProduct(lotId, monetizationID, price);
			erolabsGetBalanceRequest.GetRequest(ErolabsSDK.Token).Subscribe(delegate(ErolabsBalanceData x)
			{
				StartPayment(monetizationID, price, x.coins);
			}).AddTo(compositeDisposable);
		}
		catch (Exception ex)
		{
			Abort(ex.Message);
		}
	}

	private void StartPayment(int monetizationID, string price, int balance)
	{
		bool item = balance >= Convert.ToInt32(price);
		Action<Unit> item2 = ContinuePayment(monetizationID, price);
		Tuple<bool, int, Action<Unit>, bool> value = Tuple.Create(item, balance, item2, user.IsGuest);
		onStartBuyButton.OnNext(value);
	}

	private Action<Unit> ContinuePayment(int monetizationID, string price)
	{
		return delegate
		{
			string token = ErolabsSDK.Token;
			Transaction transaction = new Transaction(monetizationID, isValidated: false);
			checkoutRequest.Post(token, monetizationID.ToString()).Subscribe(delegate(ErolabsOrderData x)
			{
				SuccessRequest(x, transaction, monetizationID, price);
			}, delegate(Exception ex)
			{
				Abort("On create payment session " + ex.Message);
			}).AddTo(compositeDisposable);
		};
	}

	private void SuccessRequest(ErolabsOrderData erolabsOrderData, Transaction transaction, int monetizationID, string price)
	{
		transaction.IsValidated = true;
		onSucceeded.OnNext(transaction);
		hyena.Charge(erolabsOrderData.transaction_id, erolabsOrderData.payment_code, erolabsOrderData.amount, $"Lot ID {monetizationID}");
		notificationDisposable?.Dispose();
		NotifyServerAboutPurchase(erolabsOrderData).Subscribe(delegate
		{
		}).AddTo(notificationDisposable);
	}

	private bool IsRequestSuccessful(string response)
	{
		return response == "OK";
	}

	private IObservable<ErolabsOrderData> NotifyServerAboutPurchase(ErolabsOrderData erolabsOrderData)
	{
		return from _ in serverPaymentNotificator.SetRecievedNoResponse(erolabsOrderData.transaction_id).Where(IsRequestSuccessful)
			select erolabsOrderData;
	}

	private void Abort(string reason)
	{
		try
		{
			if (ParseResponseCode(reason) == 402)
			{
				onNotEnoughMoney?.OnNext(default(Unit));
			}
			else
			{
				onFailed?.OnNext("i dont know " + reason);
			}
		}
		catch (Exception)
		{
			onFailed?.OnNext(reason);
		}
	}

	private static int ParseResponseCode(string raw)
	{
		Match match = Regex.Match(raw, "ResponseCode\\s*=\\s*(\\d+)");
		if (match.Success && int.TryParse(match.Groups[1].Value, out var result))
		{
			return result;
		}
		throw new FormatException("ResponseCode not found in input string.");
	}

	public override void Dispose()
	{
		base.Dispose();
		compositeDisposable?.Dispose();
		notificationDisposable?.Dispose();
	}
}
