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
		IObservable<PaymentIntentData> emitPaymentData = Observable.Select<Response<PaymentIntentData>, PaymentIntentData>(Observable.Where<Response<PaymentIntentData>>(Observable.Where<Response<PaymentIntentData>>(Observable.SelectMany<Payment, Response<PaymentIntentData>>(Observable.Select<RawResult, Payment>(Observable.SelectMany<PaymentIntentData, RawResult>(Observable.SelectMany<Response<List<PaymentIntentData>>, PaymentIntentData>(Observable.Where<Response<List<PaymentIntentData>>>(Observable.SelectMany<User, Response<List<PaymentIntentData>>>(user.OnAuthorizedUser(), (Func<User, IObservable<Response<List<PaymentIntentData>>>>)((User _user) => suspendedPaymentsRequest.GetRequest(_user.PlayerID))), (Func<Response<List<PaymentIntentData>>, bool>)((Response<List<PaymentIntentData>> _response) => _response.Data != null && _response.Data.Any())), (Func<Response<List<PaymentIntentData>>, IEnumerable<PaymentIntentData>>)((Response<List<PaymentIntentData>> _response) => _response.Data)), (Func<PaymentIntentData, IObservable<RawResult>>)((PaymentIntentData _data) => NutakuExtensions.GetPayment(_data.InvoiceID, monoBehaviour))), (Func<RawResult, Payment>)((RawResult _response) => _response.ParseResult<Payment>())), (Func<Payment, IObservable<Response<PaymentIntentData>>>)serverNotificator.UpdatePaymentStatus), (Func<Response<PaymentIntentData>, bool>)IsRequestSuccessful), (Func<Response<PaymentIntentData>, bool>)((Response<PaymentIntentData> _response) => _response.Data.IsSucceeded)), (Func<Response<PaymentIntentData>, PaymentIntentData>)((Response<PaymentIntentData> _response) => _response.Data));
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
		return Observable.Select<Response, PaymentIntentData>(Observable.Where<Response>(serverPaymentNotificator.SetRecieved(data.ID), (Func<Response, bool>)IsRequestSuccessful), (Func<Response, PaymentIntentData>)((Response _) => data));
	}

	protected override IObservable<PaymentIntentData> AbortPurchaseNotification(PaymentIntentData data)
	{
		throw new NotImplementedException();
	}
}
