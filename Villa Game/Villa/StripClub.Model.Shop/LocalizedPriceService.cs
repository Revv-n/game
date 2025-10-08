using System.Linq;
using Zenject;

namespace StripClub.Model.Shop;

public sealed class LocalizedPriceService
{
	private readonly LocalizedPriceManager _localizedPriceManager;

	[Inject]
	private LocalizedPriceService(LocalizedPriceManager localizedPriceManager)
	{
		_localizedPriceManager = localizedPriceManager;
	}

	public int GetSelloutPoints(Lot lot)
	{
		LocalizedPrice localizedPrice = GetLocalizedPrice(lot);
		if (localizedPrice == null || !localizedPrice.IsGivingPoints || !lot.IsReal)
		{
			return 0;
		}
		return localizedPrice.PointsQty;
	}

	private LocalizedPrice GetLocalizedPrice(Lot lot)
	{
		string paymentId = string.Empty;
		if (lot is BundleLot bundleLot)
		{
			paymentId = bundleLot.PaymentID;
		}
		else if (lot is SubscriptionLot subscriptionLot)
		{
			paymentId = subscriptionLot.PaymentID;
		}
		else if (lot is GemShopLot gemShopLot)
		{
			paymentId = gemShopLot.PaymentID;
		}
		return _localizedPriceManager.Collection.FirstOrDefault((LocalizedPrice priceInfo) => priceInfo.id.Equals(paymentId));
	}
}
