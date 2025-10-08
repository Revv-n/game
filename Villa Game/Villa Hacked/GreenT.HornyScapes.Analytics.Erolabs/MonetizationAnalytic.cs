using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.Data;
using GreenT.HornyScapes.Analytics.Starshops;
using GreenT.HornyScapes.Constants;
using GreenT.HornyScapes.Monetization;
using StripClub.Model.Shop;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Analytics.Erolabs;

public class MonetizationAnalytic : BaseAnalytic<Product>
{
	private readonly IIAPController<Transaction> iapSystem;

	private readonly PlayerStats playerStats;

	private readonly StarShopAnalytic starShopAnalytic;

	private readonly LotManager lotManager;

	private readonly PlayerPaymentsStats playerPaymentsStats;

	private readonly IConstants<int> intConstants;

	private readonly LocalizedPriceService _localizedPriceService;

	private const string DOLLAR_TO_ECOIN_EXCHANGE_CONST = "dollar_to_ecoin_exchange";

	private readonly IRegionPriceResolver _priceResolver;

	public MonetizationAnalytic(IAmplitudeSender<AmplitudeEvent> amplitude, IIAPController<Transaction> iapSystem, LotManager lotManager, PlayerStats playerStats, StarShopAnalytic starShopAnalytic, PlayerPaymentsStats playerPaymentsStats, IRegionPriceResolver priceResolver, IConstants<int> intConstants, LocalizedPriceService localizedPriceService)
		: base(amplitude)
	{
		this.iapSystem = iapSystem;
		this.playerStats = playerStats;
		this.starShopAnalytic = starShopAnalytic;
		this.lotManager = lotManager;
		this.playerPaymentsStats = playerPaymentsStats;
		this.intConstants = intConstants;
		_priceResolver = priceResolver;
		_localizedPriceService = localizedPriceService;
	}

	public override void Track()
	{
		onNewStream.Clear();
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Product>(iapSystem.OnPressButton, (Action<Product>)SendPaymentCheckEvents), (ICollection<IDisposable>)onNewStream);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Transaction>(iapSystem.OnSucceeded, (Action<Transaction>)SendSucceededEvents), (ICollection<IDisposable>)onNewStream);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<string>(iapSystem.OnFailed, (Action<string>)SendFailEvents), (ICollection<IDisposable>)onNewStream);
	}

	private void SendPaymentCheckEvents(Product product)
	{
		((IAnalyticSender<AmplitudeEvent>)(object)amplitude).AddEvent((AmplitudeEvent)new PaymentCheckAmplitudeEvent(product, playerStats.CheckoutAttemptCount.Value));
	}

	private void SendSucceededEvents(Transaction data)
	{
		try
		{
			int itemId = data.monetizationID;
			Lot lot = lotManager.GetLot<Lot>().First((Lot _lot) => _lot.MonetizationID == itemId);
			if (!(lot is ValuableLot<decimal> valuableLot))
			{
				Debug.LogError($"Could not find lot for {itemId}");
				return;
			}
			decimal price = valuableLot.Price.Value / (decimal)intConstants["dollar_to_ecoin_exchange"];
			try
			{
				string paymentID = ((IPaymentID)valuableLot).PaymentID;
				price = _priceResolver.GetPriceConvertedToUS(paymentID);
			}
			catch
			{
				Debug.LogError("No paymentID for " + valuableLot.MonetizationID);
			}
			SendPurchaseEventToAmplitude(data, lot, price);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	private void SendPurchaseEventToAmplitude(Transaction data, Lot lot, decimal price)
	{
		PurchaseAmplitudeEvent purchaseAmplitudeEvent = null;
		if (lot is BundleLot || lot is GemShopLot || lot is SubscriptionLot)
		{
			int selloutPoints = _localizedPriceService.GetSelloutPoints(lot);
			purchaseAmplitudeEvent = new PurchaseAmplitudeEvent(data, price, lot, playerStats, lot.ShopSource, playerPaymentsStats.GetSumPriceAverage(), _priceResolver.CurrentRegion, selloutPoints);
		}
		if (purchaseAmplitudeEvent != null)
		{
			purchaseAmplitudeEvent.AddStarShopInfo(starShopAnalytic.LastCompletedId);
			((IAnalyticSender<AmplitudeEvent>)(object)amplitude).AddEvent((AmplitudeEvent)purchaseAmplitudeEvent);
		}
	}

	private void SendFailEvents(string reason)
	{
		((IAnalyticSender<AmplitudeEvent>)(object)amplitude).AddEvent((AmplitudeEvent)new FailAmplitudeEvent(reason));
	}
}
