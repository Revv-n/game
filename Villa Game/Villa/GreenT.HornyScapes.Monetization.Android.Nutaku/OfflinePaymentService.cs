using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Monetization.Webgl;
using GreenT.Net;
using GreenT.Nutaku;
using Nutaku.Unity;
using StripClub.Model.Shop;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Monetization.Android.Nutaku;

public class OfflinePaymentService : GreenT.HornyScapes.Monetization.Webgl.OfflinePaymentService
{
	private readonly ServerPaymentNotificator serverPaymentNotificator;

	private readonly SuspendedPaymentsRequest suspendedPaymentsRequest;

	private readonly User user;

	private readonly ServerPaymentNotificator serverNotificator;

	private readonly MonoBehaviour monoBehaviour;

	public OfflinePaymentService(SuspendedPaymentsRequest suspendedPaymentsRequest, MonoBehaviour monoBehaviour, ServerPaymentNotificator serverNotificator, User user, LotManager lotManager, GameStarter gameStarter, InvoicesFilteredRequest invoicesFilteredRequest, ServerPaymentNotificator serverPaymentNotificator, LotOfflineProvider lotOfflineProvider)
		: base(user, lotManager, lotOfflineProvider, invoicesFilteredRequest, gameStarter)
	{
		this.serverPaymentNotificator = serverPaymentNotificator;
		this.suspendedPaymentsRequest = suspendedPaymentsRequest;
		this.user = user;
		this.monoBehaviour = monoBehaviour;
		this.serverNotificator = serverNotificator;
	}

	public override void Initialize()
	{
		base.Initialize();
		ProcessSuspendedPayments();
	}

	private void ProcessSuspendedPayments()
	{
		IObservable<PaymentIntentData> emitPaymentData = from _response in (from _response in (from _response in user.OnAuthorizedUser().SelectMany((User _user) => suspendedPaymentsRequest.GetRequest(_user.PlayerID))
					where _response.Data != null && _response.Data.Any()
					select _response).SelectMany((Response<List<PaymentIntentData>> _response) => _response.Data).SelectMany((PaymentIntentData _data) => NutakuExtensions.GetPayment(_data.InvoiceID, monoBehaviour))
				select _response.ParseResult<Payment>()).SelectMany((Func<Payment, IObservable<Response<PaymentIntentData>>>)serverNotificator.UpdatePaymentStatus).Where(IsRequestSuccessful)
			where _response.Data.IsSucceeded
			select _response.Data;
		ProcessObservablePaymentData(emitPaymentData);
	}

	private bool IsRequestSuccessful(Response response)
	{
		if (response.Status != 0)
		{
			return response.Status == 200;
		}
		return true;
	}

	protected override IObservable<PaymentIntentData> NotifyServerAboutPurchase(PaymentIntentData data)
	{
		return from _ in serverPaymentNotificator.SetRecieved(data.ID).Where(IsRequestSuccessful)
			select data;
	}

	protected override IObservable<PaymentIntentData> AbortPurchaseNotification(PaymentIntentData data)
	{
		throw new NotImplementedException();
	}
}
