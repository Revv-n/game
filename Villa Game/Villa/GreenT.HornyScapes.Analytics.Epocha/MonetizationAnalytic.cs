using System;
using System.Linq;
using GreenT.HornyScapes.Analytics.Starshops;
using GreenT.HornyScapes.Monetization;
using StripClub.Model.Shop;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Analytics.Epocha;

public class MonetizationAnalytic : BaseAnalytic<Product>
{
	private readonly IIAPController<PaymentIntentData> iapSystem;

	private readonly LotManager lotManager;

	private readonly PartnerSender partnerSender;

	private readonly IEvent pixelPaymentEvent;

	private readonly PlayerStats playerStats;

	private readonly StarShopAnalytic starShopAnalytic;

	private readonly IRegionPriceResolver _priceResolver;

	private readonly PlayerPaymentsStats playerPaymentsStats;

	private readonly LocalizedPriceManager _localizedPriceManager;

	private readonly LocalizedPriceService _localizedPriceService;

	public MonetizationAnalytic(IAmplitudeSender<AmplitudeEvent> amplitude, IIAPController<PaymentIntentData> iapSystem, LotManager lotManager, PartnerSender partnerSender, [Inject(Id = "Payment")] IEvent pixelPaymentEvent, PlayerStats playerStats, PlayerPaymentsStats playerPaymentsStats, StarShopAnalytic starShopAnalytic, IRegionPriceResolver priceResolver, LocalizedPriceManager localizedPriceManager, LocalizedPriceService localizedPriceService)
		: base(amplitude)
	{
		this.partnerSender = partnerSender;
		this.pixelPaymentEvent = pixelPaymentEvent;
		this.playerStats = playerStats;
		this.starShopAnalytic = starShopAnalytic;
		_priceResolver = priceResolver;
		this.iapSystem = iapSystem;
		this.lotManager = lotManager;
		this.playerPaymentsStats = playerPaymentsStats;
		_localizedPriceManager = localizedPriceManager;
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

	private void SendPaymentCheckEvent(Product product)
	{
		SendPaymentCheckToAmplitude(product);
		SendPaymentCheckToPartner(product);
	}

	private void SendPaymentCheckToPartner(Product product)
	{
		partnerSender.AddEvent(new PaymentCheckPartnerEvent(product.ItemId, product.Price, playerStats.CheckoutAttemptCount.Value));
	}

	private void SendPaymentCheckToAmplitude(Product product)
	{
		amplitude.AddEvent(new PaymentCheckAmplitudeEvent(product, playerStats.CheckoutAttemptCount.Value));
	}

	private void SendOpenFormEvent(CheckoutData checkoutData)
	{
		SendOpenFormToGoogleAnalytic();
		SendOpenFormToAmplitude(checkoutData);
		SendOpenFormToPartner(checkoutData);
	}

	private void SendOpenFormToGoogleAnalytic()
	{
	}

	private void SendOpenFormToAmplitude(CheckoutData checkoutData)
	{
		amplitude.AddEvent(new OpenFormAmplitudeEvent(checkoutData));
	}

	private void SendOpenFormToPartner(CheckoutData checkoutData)
	{
		partnerSender.AddEvent(new PaymentCheckOpenFormPartnerEvent(checkoutData.id, checkoutData.url));
	}

	private void SendSucceededEvent(PaymentIntentData data)
	{
		try
		{
			Lot lot = lotManager.Collection.First((Lot _lot) => _lot.MonetizationID == data.ItemID);
			if (!(lot is ValuableLot<decimal> valuableLot))
			{
				Debug.LogError($"Could not find lot for {data.ItemID}");
				return;
			}
			SendPurchaseEventToGoogleAnalytic();
			SendPurchaseEventToAmplitude(data, lot, valuableLot.Price.Value);
			SendPurchaseEventToPartner(data, lot, valuableLot.Price.Value);
			SendPurchaseEventToPixel();
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	private void SendPurchaseEventToGoogleAnalytic()
	{
	}

	private void SendPurchaseEventToAmplitude(PaymentIntentData data, Lot lot, decimal price)
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
			amplitude.AddEvent(purchaseAmplitudeEvent);
		}
	}

	private void SendPurchaseEventToPartner(PaymentIntentData data, Lot lot, decimal price)
	{
		partnerSender.AddEvent(new PurchasePartnerEvent(data, lot, price));
	}

	private void SendPurchaseEventToPixel()
	{
		pixelPaymentEvent.Send();
	}

	private void SendFailEvent(string reason)
	{
		SendFailToGoogleAnalytic();
		amplitude.AddEvent(new FailAmplitudeEvent(reason));
		partnerSender.AddEvent(new FailPaymentEvent(reason));
	}

	private void SendFailToGoogleAnalytic()
	{
	}
}
