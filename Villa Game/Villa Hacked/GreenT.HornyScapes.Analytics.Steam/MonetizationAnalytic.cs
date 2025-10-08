using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Analytics.Starshops;
using GreenT.HornyScapes.Monetization;
using StripClub.Model.Shop;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Analytics.Steam;

public class MonetizationAnalytic : BaseAnalytic<Product>
{
	private readonly IIAPController<SteamPaymentData> iapSystem;

	private readonly PlayerStats playerStats;

	private readonly StarShopAnalytic starShopAnalytic;

	private readonly LotManager lotManager;

	private readonly PlayerPaymentsStats playerPaymentStats;

	private readonly IRegionPriceResolver _priceResolver;

	private readonly LocalizedPriceService _localizedPriceService;

	public MonetizationAnalytic(IAmplitudeSender<AmplitudeEvent> amplitude, IIAPController<SteamPaymentData> iapSystem, LotManager lotManager, PlayerStats playerStats, StarShopAnalytic starShopAnalytic, PlayerPaymentsStats playerPaymentsStats, IRegionPriceResolver priceResolver, LocalizedPriceService localizedPriceService)
		: base(amplitude)
	{
		this.iapSystem = iapSystem;
		this.playerStats = playerStats;
		this.starShopAnalytic = starShopAnalytic;
		this.lotManager = lotManager;
		playerPaymentStats = playerPaymentsStats;
		_priceResolver = priceResolver;
		_localizedPriceService = localizedPriceService;
	}

	public override void Track()
	{
		onNewStream.Clear();
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Product>(iapSystem.OnPressButton, (Action<Product>)SendPaymentCheckEvents), (ICollection<IDisposable>)onNewStream);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<SteamPaymentData>(iapSystem.OnSucceeded, (Action<SteamPaymentData>)SendSucceededEvents), (ICollection<IDisposable>)onNewStream);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<string>(iapSystem.OnFailed, (Action<string>)SendFailEvents), (ICollection<IDisposable>)onNewStream);
	}

	private void SendPaymentCheckEvents(Product product)
	{
		((IAnalyticSender<AmplitudeEvent>)(object)amplitude).AddEvent((AmplitudeEvent)new PaymentCheckAmplitudeEvent(product, playerStats.CheckoutAttemptCount.Value));
	}

	private void SendSucceededEvents(SteamPaymentData data)
	{
		try
		{
			int.TryParse(data.item_id, out var itemId);
			Lot lot = lotManager.GetLot<Lot>().First((Lot _lot) => _lot.MonetizationID == itemId);
			if (!(lot is ValuableLot<decimal> valuableLot))
			{
				Debug.LogError($"Could not find lot for {itemId}");
			}
			else
			{
				SendPurchaseEventToAmplitude(data, lot, valuableLot.Price.Value);
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	private void SendPurchaseEventToAmplitude(SteamPaymentData data, Lot lot, decimal price)
	{
		int selloutPoints = _localizedPriceService.GetSelloutPoints(lot);
		PurchaseAmplitudeEvent purchaseAmplitudeEvent = new PurchaseAmplitudeEvent(data, price, lot, playerStats, lot.ShopSource, playerPaymentStats.GetSumPriceAverage(), _priceResolver.CurrentRegion, selloutPoints);
		purchaseAmplitudeEvent.AddStarShopInfo(starShopAnalytic.LastCompletedId);
		((IAnalyticSender<AmplitudeEvent>)(object)amplitude).AddEvent((AmplitudeEvent)purchaseAmplitudeEvent);
	}

	private void SendFailEvents(string reason)
	{
		((IAnalyticSender<AmplitudeEvent>)(object)amplitude).AddEvent((AmplitudeEvent)new FailAmplitudeEvent(reason));
	}
}
