using System;
using GreenT.Data;
using GreenT.HornyScapes.Analytics;
using GreenT.Types;
using StripClub.Model;
using StripClub.Model.Shop;
using StripClub.Model.Shop.Data;
using Zenject;

namespace GreenT.HornyScapes.Bank;

public class SummonLotFactory : LotFactory<SummonMapper, SummonLot>
{
	private readonly LinkedContentAnalyticDataFactory analyticDataFactory;

	private readonly IFactory<int, LinkedContentAnalyticData, LootboxLinkedContent> lootboxContentFactory;

	private readonly Func<DateTime> getTimeFunc;

	public SummonLotFactory(LockerFactory lockerFactory, ISaver saver, IPurchaseProcessor purchaseProcessor, LinkedContentAnalyticDataFactory analyticDataFactory, IFactory<int, LinkedContentAnalyticData, LootboxLinkedContent> lootboxContentFactory, IClock clock)
		: base(lockerFactory, saver, purchaseProcessor)
	{
		this.analyticDataFactory = analyticDataFactory;
		this.lootboxContentFactory = lootboxContentFactory;
		getTimeFunc = clock.GetTime;
	}

	public override SummonLot Create(SummonMapper mapper)
	{
		EqualityLocker<int> countLocker;
		ILocker locker = CreateCompositeLocker(mapper, out countLocker, LockerSourceType.SummonLot);
		(CurrencyType, CompositeIdentificator) tuple = PriceResourceHandler.ParsePriceSourse(mapper.price_resource);
		Price<int> price = new Price<int>(mapper.price, tuple.Item1, tuple.Item2);
		LinkedContentAnalyticData param = analyticDataFactory.Create(CurrencyAmplitudeAnalytic.SourceType.Summon);
		LinkedContent reward = lootboxContentFactory.Create(mapper.reward, param);
		LinkedContent firstPurchaseReward = (mapper.first_reward.HasValue ? lootboxContentFactory.Create(mapper.first_reward.Value, param) : null);
		SummonLot.RewardSettings singleReward = new SummonLot.RewardSettings(reward, firstPurchaseReward, price);
		price = (mapper.price_x10.HasValue ? new Price<int>(mapper.price_x10.Value, tuple.Item1, tuple.Item2) : null);
		reward = (mapper.reward_10.HasValue ? lootboxContentFactory.Create(mapper.reward_10.Value, param) : null);
		firstPurchaseReward = (mapper.first_reward_10.HasValue ? lootboxContentFactory.Create(mapper.first_reward_10.Value, param) : null);
		SummonLot.RewardSettings wholesaleReward = new SummonLot.RewardSettings(reward, firstPurchaseReward, price);
		SummonLot summonLot = new SummonLot(mapper, singleReward, wholesaleReward, locker, countLocker, purchaseProcessor, getTimeFunc);
		saver.Add(summonLot);
		return summonLot;
	}
}
