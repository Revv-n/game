using System;
using System.Linq;
using GreenT.HornyScapes.Analytics.Starshops;
using GreenT.HornyScapes.Monetization;
using StripClub.Model.Shop;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Analytics.Harem;

public class MonetizationAnalytic : BaseAnalytic<Product>
{
	private readonly IIAPController<Transaction> iapSystem;

	private readonly PlayerStats playerStats;

	private readonly StarShopAnalytic starShopAnalytic;

	private readonly LotManager lotManager;

	private readonly PlayerPaymentsStats playerPaymentsStats;

	private readonly IRegionPriceResolver _priceResolver;

	private readonly LocalizedPriceService _localizedPriceService;

	public MonetizationAnalytic(IAmplitudeSender<AmplitudeEvent> amplitude, IIAPController<Transaction> iapSystem, LotManager lotManager, PlayerStats playerStats, StarShopAnalytic starShopAnalytic, PlayerPaymentsStats playerPaymentsStats, IRegionPriceResolver priceResolver, LocalizedPriceService localizedPriceService)
		: base(amplitude)
	{
		this.iapSystem = iapSystem;
		this.playerStats = playerStats;
		this.starShopAnalytic = starShopAnalytic;
		this.lotManager = lotManager;
		this.playerPaymentsStats = playerPaymentsStats;
		_priceResolver = priceResolver;
		_localizedPriceService = localizedPriceService;
	}

	public override void Track()
	{
		onNewStream.Clear();
		iapSystem.OnPressButton.Subscribe(SendPaymentCheckEvent).AddTo(onNewStream);
		iapSystem.OnOpenForm.Subscribe(SendOpenFormEvent).AddTo(onNewStream);
		iapSystem.OnSucceeded.Subscribe(SendSucceededEvent).AddTo(onNewStream);
		iapSystem.OnFailed.Subscribe(SendFailEvent).AddTo(onNewStream);
	}

	private void SendSucceededEvent(Transaction data)
	{
		try
		{
			int itemId = data.monetizationID;
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

	private void SendPurchaseEventToAmplitude(Transaction data, Lot lot, decimal price)
	{
		PurchaseAmplitudeEvent purchaseAmplitudeEvent = null;
		if (lot is BundleLot || lot is GemShopLot)
		{
			int selloutPoints = _localizedPriceService.GetSelloutPoints(lot);
			purchaseAmplitudeEvent = new PurchaseAmplitudeEvent(data, price, lot, playerStats, lot.ShopSource, playerPaymentsStats.GetSumPriceAverage(), _priceResolver.CurrentRegion, selloutPoints);
		}
		if (purchaseAmplitudeEvent != null)
		{
			purchaseAmplitudeEvent.AddStarShopInfo(starShopAnalytic.LastCompletedId);
			amplitude.AddEvent(purchaseAmplitudeEvent);
		}
	}

	private void SendFailEvent(string reason)
	{
		amplitude.AddEvent(new FailAmplitudeEvent(reason));
	}

	private void SendPaymentCheckEvent(Product product)
	{
		SendPaymentCheckToAmplitude(product);
	}

	private void SendOpenFormEvent(CheckoutData checkoutData)
	{
		SendOpenFormToAmplitude(checkoutData);
	}

	private void SendPaymentCheckToAmplitude(Product product)
	{
		amplitude.AddEvent(new PaymentCheckAmplitudeEvent(product, playerStats.CheckoutAttemptCount.Value));
	}

	private void SendOpenFormToAmplitude(CheckoutData checkoutData)
	{
		amplitude.AddEvent(new OpenFormAmplitudeEvent(checkoutData));
	}
}
