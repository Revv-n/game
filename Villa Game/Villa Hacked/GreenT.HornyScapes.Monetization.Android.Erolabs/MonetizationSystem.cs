using System;
using System.Collections.Generic;
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

	public IObservable<Unit> OnSuccess => Observable.Select<Transaction, Unit>(OnSucceeded, (Func<Transaction, Unit>)((Transaction _) => default(Unit)));

	public virtual IObservable<Tuple<bool, int, Action<Unit>, bool>> OnStartBuyButton => Observable.AsObservable<Tuple<bool, int, Action<Unit>, bool>>((IObservable<Tuple<bool, int, Action<Unit>, bool>>)onStartBuyButton);

	public virtual IObservable<Unit> OnNotEnoughMoney => Observable.AsObservable<Unit>((IObservable<Unit>)onNotEnoughMoney);

	public MonetizationSystem(ErolabsCheckoutRequest checkoutRequest, ErolabsInvoicesFilteredRequest invoicesFilteredRequest, Hyena hyena, ServerPaymentNotificator serverPaymentNotificator, ErolabsGetBalanceRequest erolabsGetBalanceRequest, User user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
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
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<ErolabsBalanceData>(erolabsGetBalanceRequest.GetRequest(ErolabsSDK.Token), (Action<ErolabsBalanceData>)delegate(ErolabsBalanceData x)
			{
				StartPayment(monetizationID, price, x.coins);
			}), (ICollection<IDisposable>)compositeDisposable);
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
		Tuple<bool, int, Action<Unit>, bool> tuple = Tuple.Create(item, balance, item2, user.IsGuest);
		onStartBuyButton.OnNext(tuple);
	}

	private Action<Unit> ContinuePayment(int monetizationID, string price)
	{
		return delegate
		{
			string token = ErolabsSDK.Token;
			Transaction transaction = new Transaction(monetizationID, isValidated: false);
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<ErolabsOrderData>(checkoutRequest.Post(token, monetizationID.ToString()), (Action<ErolabsOrderData>)delegate(ErolabsOrderData x)
			{
				SuccessRequest(x, transaction, monetizationID, price);
			}, (Action<Exception>)delegate(Exception ex)
			{
				Abort("On create payment session " + ex.Message);
			}), (ICollection<IDisposable>)compositeDisposable);
		};
	}

	private void SuccessRequest(ErolabsOrderData erolabsOrderData, Transaction transaction, int monetizationID, string price)
	{
		transaction.IsValidated = true;
		onSucceeded.OnNext(transaction);
		hyena.Charge(erolabsOrderData.transaction_id, erolabsOrderData.payment_code, erolabsOrderData.amount, $"Lot ID {monetizationID}");
		CompositeDisposable obj = notificationDisposable;
		if (obj != null)
		{
			obj.Dispose();
		}
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<ErolabsOrderData>(NotifyServerAboutPurchase(erolabsOrderData), (Action<ErolabsOrderData>)delegate
		{
		}), (ICollection<IDisposable>)notificationDisposable);
	}

	private bool IsRequestSuccessful(string response)
	{
		return response == "OK";
	}

	private IObservable<ErolabsOrderData> NotifyServerAboutPurchase(ErolabsOrderData erolabsOrderData)
	{
		return Observable.Select<string, ErolabsOrderData>(Observable.Where<string>(serverPaymentNotificator.SetRecievedNoResponse(erolabsOrderData.transaction_id), (Func<string, bool>)IsRequestSuccessful), (Func<string, ErolabsOrderData>)((string _) => erolabsOrderData));
	}

	private void Abort(string reason)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
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
		CompositeDisposable obj = compositeDisposable;
		if (obj != null)
		{
			obj.Dispose();
		}
		CompositeDisposable obj2 = notificationDisposable;
		if (obj2 != null)
		{
			obj2.Dispose();
		}
	}
}
