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
		return (from _response in user.OnAuthorizedUser().CombineLatest(_gameReadyToCheck, (User user, Unit configLoaded) => user).SelectMany((User _user) => _invoicesFilteredRequestNoResponse.GetRequest(_user.PlayerID))
				.Debug("OfflinePayment: Check invoice payments", LogType.Payments)
			where _response?.Any() ?? false
			select from x in _response
				where x.TransactionId != ""
				select x into _response
			select (_response)).SelectMany((IEnumerable<PaymentIntentData> x) => x);
	}

	private bool IsRequestSuccessful(string response)
	{
		return response == "OK";
	}

	protected override IObservable<PaymentIntentData> NotifyServerAboutPurchase(PaymentIntentData data)
	{
		return from _ in serverPaymentNotificator.SetRecievedNoResponse(data.TransactionId).Where(IsRequestSuccessful)
			select data;
	}

	protected override IObservable<PaymentIntentData> AbortPurchaseNotification(PaymentIntentData data)
	{
		throw new NotImplementedException();
	}
}
