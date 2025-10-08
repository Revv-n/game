using System;
using Steamworks;
using UniRx;

namespace GreenT.HornyScapes.Monetization.Windows.Steam;

public class MonetizationSystem : MonetizationSystem<SteamPaymentData>, IMonetizationAdapter
{
	private readonly SteamCheckoutRequest _checkoutRequest;

	private readonly SteamConfirmRequest _confirmRequest;

	private readonly MonetizationRecorder _monetizationRecorder;

	private CompositeDisposable stream = new CompositeDisposable();

	public IObservable<Unit> OnSuccess => OnSucceeded.Select((SteamPaymentData _) => default(Unit));

	public MonetizationSystem(SteamCheckoutRequest checkoutRequest, SteamConfirmRequest confirmRequest, MonetizationRecorder monetizationRecorder)
	{
		_checkoutRequest = checkoutRequest;
		_confirmRequest = confirmRequest;
		_monetizationRecorder = monetizationRecorder;
	}

	public void BuyProduct(string lotId, int monetizationID, int entityId, string price = null, string region = null, string itemName = null, string itemDescription = null, string itemImageUrl = null, string currenyType = null)
	{
		BuyProduct(lotId, monetizationID, price);
		_checkoutRequest.Post(lotId, region, monetizationID.ToString(), itemDescription, currenyType).Subscribe(delegate
		{
		}, delegate(Exception ex)
		{
			Abort("On create payment session " + ex.Message);
		}).AddTo(stream);
	}

	public void TxnAuthCallback(MicroTxnAuthorizationResponse_t response)
	{
		if (response.m_bAuthorized != 1)
		{
			Abort("Steam failed to authorize payment");
			return;
		}
		_confirmRequest.Post(response.m_ulOrderID.ToString(), unreceived: true).Do(_monetizationRecorder.Record).Subscribe(OnSuccessConfirm, delegate(Exception ex)
		{
			Abort("On confirm the payment " + ex.Message);
		})
			.AddTo(stream);
	}

	private void OnSuccessConfirm(SteamPaymentData data)
	{
		onSucceeded.OnNext(data);
	}

	private void Abort(string reason)
	{
		onFailed.OnNext(reason);
	}
}
