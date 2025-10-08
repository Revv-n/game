using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Extensions;
using StripClub.Model.Shop;

namespace GreenT.HornyScapes.Monetization.Windows;

public abstract class BasePriceResolver : IRegionPriceResolver
{
	private readonly LotManager _lotManager;

	private readonly LocalizedPriceManager _localizedPriceManager;

	public string CurrentRegion { get; private set; }

	public IDictionary<string, decimal> Prices { get; } = new Dictionary<string, decimal>();


	public abstract void Initialize();

	protected BasePriceResolver(LotManager lotManager, LocalizedPriceManager localizedPriceManager)
	{
		_lotManager = lotManager;
		_localizedPriceManager = localizedPriceManager;
	}

	public void UpdatePrices(string region)
	{
		Dictionary<string, decimal> prices = GetPrices(region);
		foreach (ValuableLot<decimal> item in _lotManager.Collection.OfType<ValuableLot<decimal>>())
		{
			if (item.IsReal)
			{
				string text = ((item is GemShopLot gemShopLot) ? gemShopLot.PaymentID : ((item is BundleLot bundleLot) ? bundleLot.PaymentID : ((!(item is SubscriptionLot subscriptionLot)) ? "" : subscriptionLot.PaymentID)));
				string key = text;
				decimal value = prices[key];
				item.Price.Value = value;
			}
		}
	}

	public decimal GetPriceConvertedToUS(string paymentID)
	{
		return GetPrices("US")[paymentID];
	}

	protected void Setup(string region)
	{
		Dictionary<string, decimal> prices = GetPrices(region);
		Prices.AddRange(prices);
		CurrentRegion = region;
	}

	private Dictionary<string, decimal> GetPrices(string region)
	{
		return _localizedPriceManager.Collection.Select((LocalizedPrice item) => (id: item.id, GetPriceByRegion(region, item))).ToDictionary(((string id, decimal) x) => x.id, ((string id, decimal) x) => x.Item2);
	}

	private decimal GetPriceByRegion(string region, LocalizedPrice localizedPrice)
	{
		return region switch
		{
			"US" => localizedPrice.US, 
			"RU" => localizedPrice.RU, 
			"ES" => localizedPrice.ES, 
			"PT" => localizedPrice.PT, 
			"DE" => localizedPrice.DE, 
			"FR" => localizedPrice.FR, 
			"IT" => localizedPrice.IT, 
			"PL" => localizedPrice.PL, 
			"BR" => localizedPrice.BR, 
			"IN" => localizedPrice.IN, 
			"MX" => localizedPrice.MX, 
			"HK" => localizedPrice.HK, 
			"TW" => localizedPrice.TW, 
			"CA" => localizedPrice.CA, 
			"UA" => localizedPrice.UA, 
			"AU" => localizedPrice.AU, 
			"VN" => localizedPrice.VN, 
			"TH" => localizedPrice.TH, 
			"JP" => localizedPrice.JP, 
			"KZ" => localizedPrice.KZ, 
			"nutaku" => localizedPrice.nutaku, 
			"erolabs" => localizedPrice.erolabs, 
			_ => localizedPrice.US, 
		};
	}
}
