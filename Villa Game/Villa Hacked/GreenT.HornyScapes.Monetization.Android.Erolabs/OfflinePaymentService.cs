using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Monetization.Webgl;
using StripClub.Model.Shop;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Monetization.Android.Erolabs;

public class OfflinePaymentService : GreenT.HornyScapes.Monetization.Webgl.OfflinePaymentService
{
	private readonly ServerPaymentNotificator serverPaymentNotificator;

	private readonly SuspendedPaymentsRequest suspendedPaymentsRequest;

	private readonly User user;

	private readonly ServerPaymentNotificator serverNotificator;

	private readonly MonoBehaviour monoBehaviour;

	private readonly InvoicesFilteredRequestNoResponse _invoicesFilteredRequestNoResponse;

	public OfflinePaymentService(SuspendedPaymentsRequest suspendedPaymentsRequest, MonoBehaviour monoBehaviour, ServerPaymentNotificator serverNotificator, User user, LotManager lotManager, GameStarter gameStarter, InvoicesFilteredRequest invoicesFilteredRequest, InvoicesFilteredRequestNoResponse invoicesFilteredRequestNoResponse, ServerPaymentNotificator serverPaymentNotificator, LotOfflineProvider lotOfflineProvider)
		: base(user, lotManager, lotOfflineProvider, invoicesFilteredRequest, gameStarter)
	{
		this.serverPaymentNotificator = serverPaymentNotificator;
		this.suspendedPaymentsRequest = suspendedPaymentsRequest;
		this.user = user;
		this.monoBehaviour = monoBehaviour;
		this.serverNotificator = serverNotificator;
		_invoicesFilteredRequestNoResponse = invoicesFilteredRequestNoResponse;
	}

	protected override IObservable<PaymentIntentData> OnGetPaymentsObservable()
	{
		return Observable.SelectMany<IEnumerable<PaymentIntentData>, PaymentIntentData>(Observable.Select<IEnumerable<PaymentIntentData>, IEnumerable<PaymentIntentData>>(Observable.Select<List<PaymentIntentData>, IEnumerable<PaymentIntentData>>(Observable.Where<List<PaymentIntentData>>(Observable.SelectMany<User, List<PaymentIntentData>>(Observable.CombineLatest<User, Unit, User>(user.OnAuthorizedUser(), (IObservable<Unit>)_gameReadyToCheck, (Func<User, Unit, User>)((User user, Unit configLoaded) => user)), (Func<User, IObservable<List<PaymentIntentData>>>)((User _user) => _invoicesFilteredRequestNoResponse.GetRequest(_user.PlayerID))).Debug("OfflinePayment: Check invoice payments", LogType.Payments), (Func<List<PaymentIntentData>, bool>)((List<PaymentIntentData> _response) => _response?.Any() ?? false)), (Func<List<PaymentIntentData>, IEnumerable<PaymentIntentData>>)((List<PaymentIntentData> _response) => _response.Where((PaymentIntentData x) => x.TransactionId != ""))), (Func<IEnumerable<PaymentIntentData>, IEnumerable<PaymentIntentData>>)((IEnumerable<PaymentIntentData> _response) => _response)), (Func<IEnumerable<PaymentIntentData>, IEnumerable<PaymentIntentData>>)((IEnumerable<PaymentIntentData> x) => x));
	}

	private bool IsRequestSuccessful(string response)
	{
		return response == "OK";
	}

	protected override IObservable<PaymentIntentData> NotifyServerAboutPurchase(PaymentIntentData data)
	{
		return Observable.Select<string, PaymentIntentData>(Observable.Where<string>(serverPaymentNotificator.SetRecievedNoResponse(data.TransactionId), (Func<string, bool>)IsRequestSuccessful), (Func<string, PaymentIntentData>)((string _) => data));
	}

	protected override IObservable<PaymentIntentData> AbortPurchaseNotification(PaymentIntentData data)
	{
		throw new NotImplementedException();
	}
}
