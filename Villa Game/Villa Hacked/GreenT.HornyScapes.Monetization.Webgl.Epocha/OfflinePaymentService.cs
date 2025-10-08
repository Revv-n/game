using System;
using System.Collections.Generic;
using GreenT.Net;
using StripClub.Model.Shop;
using UniRx;

namespace GreenT.HornyScapes.Monetization.Webgl.Epocha;

public class OfflinePaymentService : GreenT.HornyScapes.Monetization.Webgl.OfflinePaymentService
{
	private readonly MonetizationSubsystem monetizationSubsystem;

	private readonly AbortRequest updateRequest;

	public OfflinePaymentService(MonetizationSubsystem monetizationSubsystem, GameStarter gameStarter, User user, LotManager lotManager, InvoicesFilteredRequest invoicesFilteredRequest, AbortRequest updatePaymentStatusRequest, LotOfflineProvider lotOfflineProvider)
		: base(user, lotManager, lotOfflineProvider, invoicesFilteredRequest, gameStarter)
	{
		this.monetizationSubsystem = monetizationSubsystem;
		updateRequest = updatePaymentStatusRequest;
	}

	protected override void OnIssueTheProductBought(PaymentIntentData data)
	{
		base.OnIssueTheProductBought(data);
		monetizationSubsystem.OnIssueTheProductBought(data);
	}

	protected override IObservable<PaymentIntentData> AbortPurchaseNotification(PaymentIntentData data)
	{
		IDictionary<string, string> fields = new Dictionary<string, string>();
		return Observable.Select<Response, PaymentIntentData>(updateRequest.Post(fields, data.ID), (Func<Response, PaymentIntentData>)((Response _) => data));
	}

	protected override IObservable<PaymentIntentData> NotifyServerAboutPurchase(PaymentIntentData data)
	{
		return Observable.Select<Response, PaymentIntentData>(Observable.Where<Response>(monetizationSubsystem.SendReceivedRequest(data), (Func<Response, bool>)((Response _response) => _response.Status == 200)), (Func<Response, PaymentIntentData>)((Response _) => data));
	}
}
