using System.Linq;
using GreenT.Data;
using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.Monetization;
using GreenT.HornyScapes.Monetization.Webgl;
using GreenT.Types;
using ModestTree;
using StripClub.Model;
using StripClub.Model.Shop;
using Zenject;

namespace GreenT.HornyScapes.Bank;

public class SubscriptionLotFactory : LotFactory<SubscriptionLotMapper, SubscriptionLot>
{
	private readonly IClock _clock;

	private readonly LinkedContentAnalyticDataFactory _analyticDataFactory;

	private readonly IFactory<int, LinkedContentAnalyticData, LootboxLinkedContent> _lootboxContentFactory;

	private readonly IRegionPriceResolver _priceResolver;

	private readonly PriceChecker _priceChecker;

	private readonly BundlesProviderBase _bundlesProviderBase;

	public SubscriptionLotFactory(LockerFactory lockerFactory, ISaver saver, IPurchaseProcessor purchaseProcessor, IClock clock, BundlesProviderBase bundlesProviderBase, LinkedContentAnalyticDataFactory analyticDataFactory, IFactory<int, LinkedContentAnalyticData, LootboxLinkedContent> lootboxContentFactory, IRegionPriceResolver priceResolver, [InjectOptional] PriceChecker priceChecker)
		: base(lockerFactory, saver, purchaseProcessor)
	{
		_clock = clock;
		_analyticDataFactory = analyticDataFactory;
		_lootboxContentFactory = lootboxContentFactory;
		_priceResolver = priceResolver;
		_priceChecker = priceChecker;
		_bundlesProviderBase = bundlesProviderBase;
	}

	public override SubscriptionLot Create(SubscriptionLotMapper mapper)
	{
		EqualityLocker<int> countLocker;
		CompositeLocker locker = CreateCompositeLocker(mapper, out countLocker, LockerSourceType.BundleLot);
		LinkedContent linkedContent = SetupRewards(mapper.lootbox_id);
		LinkedContent linkedContent2 = SetupRewards(mapper.booster_lootbox_id);
		LinkedContent rechargeReward = SetupRewards(mapper.recharge_lootbox_id);
		LinkedContent linkedContent3 = linkedContent.Clone();
		if (linkedContent2 != null)
		{
			linkedContent3.Insert(linkedContent2);
		}
		SubscriptionLot.ViewSettings viewSettings = new SubscriptionLot.ViewSettings(_bundlesProviderBase, mapper.content_source, mapper.view_prefab, mapper.view_parameters);
		MonetizationRequestData data = new MonetizationRequestData(mapper.item_name, mapper.item_descr, mapper.image_name);
		(CurrencyType, CompositeIdentificator) tuple = PriceResourceHandler.ParsePriceSourse(mapper.price_resource);
		decimal price = ((tuple.Item1 == CurrencyType.Real) ? _priceResolver.Prices[mapper.lot_id] : mapper.price);
		SubscriptionLot subscriptionLot = new SubscriptionLot(mapper, viewSettings, locker, linkedContent3, linkedContent, linkedContent2, rechargeReward, countLocker, purchaseProcessor, price, _clock.GetTime, data, tuple.Item1, tuple.Item2);
		saver.Add(subscriptionLot);
		return subscriptionLot;
	}

	private LinkedContent SetupRewards(int[] lootboxIDs)
	{
		if (lootboxIDs == null || lootboxIDs.IsEmpty())
		{
			return null;
		}
		LinkedContentAnalyticData param = _analyticDataFactory.Create(CurrencyAmplitudeAnalytic.SourceType.None);
		LootboxLinkedContent lootboxLinkedContent = _lootboxContentFactory.Create(lootboxIDs.First(), param);
		if (lootboxIDs.Length <= 1)
		{
			return lootboxLinkedContent;
		}
		foreach (int param2 in lootboxIDs)
		{
			LinkedContentAnalyticData param3 = _analyticDataFactory.Create(CurrencyAmplitudeAnalytic.SourceType.None);
			LootboxLinkedContent content = _lootboxContentFactory.Create(param2, param3);
			lootboxLinkedContent.Insert(content);
		}
		return lootboxLinkedContent;
	}
}
