using GreenT.Data;
using GreenT.HornyScapes;
using GreenT.HornyScapes.Monetization.Webgl;
using GreenT.Types;
using StripClub.Model.Shop.Data;
using UnityEngine;

namespace StripClub.Model.Shop;

public class GemShopLot : ValuableLot<decimal>, IPaymentID
{
	private readonly BundlesProviderBase _bundlesProvider;

	private readonly ContentSource _contentSource;

	public readonly MonetizationRequestData Data;

	private string uniqueKey;

	public override string LocalizationKey { get; }

	public Stickers Stickers { get; }

	public int? SaleValue { get; }

	public string PaymentID { get; }

	public LinkedContent Reward { get; }

	public LinkedContent ExtraReward { get; }

	public override LinkedContent Content { get; }

	public override Price<decimal> Price { get; }

	public Price<decimal> OldPrice { get; }

	public int? OldCount { get; }

	public string IconName { get; }

	public override bool IsFree => Price.Value == 0m;

	public GemShopLot(GemShopMapper mapper, BundlesProviderBase bundlesProvider, ContentSource contentSource, LinkedContent reward, LinkedContent extraReward, ILocker locker, EqualityLocker<int> countLocker, decimal price, IPurchaseProcessor purchaseProcessor, MonetizationRequestData data, CurrencyType currencyType, CompositeIdentificator compositeIdentificator)
		: base(mapper.id, mapper.monetization_id, mapper.tab_id, mapper.position, mapper.buy_times, locker, countLocker, purchaseProcessor, mapper.source)
	{
		LocalizationKey = mapper.local_key ?? ("content.shop.gem." + base.ID);
		Stickers = (Stickers)0;
		if (mapper.first_purchase)
		{
			Stickers |= Stickers.FirstPurchase;
		}
		if (mapper.hot)
		{
			Stickers |= Stickers.Hot;
		}
		if (mapper.best)
		{
			Stickers |= Stickers.Best;
		}
		if (mapper.sale)
		{
			Stickers |= Stickers.Sale;
		}
		_bundlesProvider = bundlesProvider;
		_contentSource = contentSource;
		SaleValue = mapper.sale_value;
		PaymentID = mapper.lot_id;
		Reward = reward;
		ExtraReward = extraReward;
		Reward.Insert(ExtraReward);
		Content = Reward;
		Price = new Price<decimal>(price, currencyType, compositeIdentificator);
		if (mapper.prev_price.HasValue)
		{
			OldPrice = new Price<decimal>(mapper.prev_price.Value, currencyType, compositeIdentificator);
		}
		OldCount = mapper.prev_count;
		IconName = mapper.icon;
		Data = data;
		uniqueKey = "lot.gemshop." + base.ID;
	}

	public override string UniqueKey()
	{
		return uniqueKey;
	}

	public Sprite GetIcon()
	{
		return _bundlesProvider.TryFindInConcreteBundle<Sprite>(_contentSource, IconName);
	}
}
