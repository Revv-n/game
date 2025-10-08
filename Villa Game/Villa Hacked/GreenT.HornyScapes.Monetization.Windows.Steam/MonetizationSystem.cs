using System;
using System.Collections.Generic;
using GreenT.Net;
using Steamworks;
using UniRx;

namespace GreenT.HornyScapes.Monetization.Windows.Steam;

public class MonetizationSystem : MonetizationSystem<SteamPaymentData>, IMonetizationAdapter
{
	private readonly SteamCheckoutRequest _checkoutRequest;

	private readonly SteamConfirmRequest _confirmRequest;

	private readonly MonetizationRecorder _monetizationRecorder;

	private CompositeDisposable stream = new CompositeDisposable();

	public IObservable<Unit> OnSuccess => Observable.Select<SteamPaymentData, Unit>(OnSucceeded, (Func<SteamPaymentData, Unit>)((SteamPaymentData _) => default(Unit)));

	public MonetizationSystem(SteamCheckoutRequest checkoutRequest, SteamConfirmRequest confirmRequest, MonetizationRecorder monetizationRecorder)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		_checkoutRequest = checkoutRequest;
		_confirmRequest = confirmRequest;
		_monetizationRecorder = monetizationRecorder;
	}

	public void BuyProduct(string lotId, int monetizationID, int entityId, string price = null, string region = null, string itemName = null, string itemDescription = null, string itemImageUrl = null, string currenyType = null)
	{
		BuyProduct(lotId, monetizationID, price);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Response>(_checkoutRequest.Post(lotId, region, monetizationID.ToString(), itemDescription, currenyType), (Action<Response>)delegate
		{
		}, (Action<Exception>)delegate(Exception ex)
		{
			Abort("On create payment session " + ex.Message);
		}), (ICollection<IDisposable>)stream);
	}

	public void TxnAuthCallback(MicroTxnAuthorizationResponse_t response)
	{
		if (response.m_bAuthorized != 1)
		{
			Abort("Steam failed to authorize payment");
			return;
		}
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<SteamPaymentData>(Observable.Do<SteamPaymentData>(_confirmRequest.Post(response.m_ulOrderID.ToString(), unreceived: true), (Action<SteamPaymentData>)_monetizationRecorder.Record), (Action<SteamPaymentData>)OnSuccessConfirm, (Action<Exception>)delegate(Exception ex)
		{
			Abort("On confirm the payment " + ex.Message);
		}), (ICollection<IDisposable>)stream);
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
