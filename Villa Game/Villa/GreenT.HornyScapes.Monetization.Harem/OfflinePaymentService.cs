using System;
using GreenT.HornyScapes.Monetization.Webgl;
using GreenT.Net;
using StripClub.Model.Shop;
using UniRx;

namespace GreenT.HornyScapes.Monetization.Harem;

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
		return from _ in updateRequest.Get(data.InvoiceID, PaymentIntentData.PaymentStatus.aborted)
			select data;
	}

	protected override IObservable<PaymentIntentData> NotifyServerAboutPurchase(PaymentIntentData data)
	{
		return from _response in receivedRequest.PostWithEmptyFields(data.ID)
			where _response.Status == 200
			select _response into _
			select data;
	}
}
