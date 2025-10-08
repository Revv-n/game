using System;
using GreenT.Net;
using StripClub.Model.Shop;
using UniRx;

namespace GreenT.HornyScapes.Monetization.Webgl.Nutaku;

public class OfflinePaymentService : GreenT.HornyScapes.Monetization.Webgl.OfflinePaymentService
{
	private readonly ReceivedRequest receivedRequest;

	private readonly PaymentStatusUpdateRequest updateRequest;

	private readonly MonetizationSystem _monetizationSystem;

	public OfflinePaymentService(User user, InvoicesFilteredRequest invoicesFilteredRequest, GameStarter gameStarter, LotManager lotManager, ReceivedRequest receivedRequest, PaymentStatusUpdateRequest updatePaymentStatusRequest, MonetizationSystem monetizationSystem, LotOfflineProvider lotOfflineProvider)
		: base(user, lotManager, lotOfflineProvider, invoicesFilteredRequest, gameStarter)
	{
		this.receivedRequest = receivedRequest;
		updateRequest = updatePaymentStatusRequest;
		_monetizationSystem = monetizationSystem;
	}

	protected override void OnIssueTheProductBought(PaymentIntentData data)
	{
		base.OnIssueTheProductBought(data);
		_monetizationSystem.OnIssueTheProductBought(data);
	}

	protected override IObservable<PaymentIntentData> AbortPurchaseNotification(PaymentIntentData data)
	{
		return Observable.Select<Response, PaymentIntentData>(updateRequest.Get(data.InvoiceID, PaymentIntentData.PaymentStatus.aborted), (Func<Response, PaymentIntentData>)((Response _) => data));
	}

	protected override IObservable<PaymentIntentData> NotifyServerAboutPurchase(PaymentIntentData data)
	{
		return Observable.Select<Response, PaymentIntentData>(Observable.Where<Response>(receivedRequest.PostWithEmptyFields(data.ID), (Func<Response, bool>)((Response _response) => _response.Status == 200)), (Func<Response, PaymentIntentData>)((Response _) => data));
	}
}
