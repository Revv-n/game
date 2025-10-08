using System.Collections.Generic;
using GreenT.Data;
using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.Lootboxes;
using GreenT.HornyScapes.MiniEvents;
using GreenT.Types;
using StripClub.Model;
using StripClub.Model.Shop;
using Zenject;

namespace GreenT.HornyScapes;

public abstract class RouletteBaseFactory<T, P> : IFactory<T, P>, IFactory where T : RouletteSummonMapper where P : RouletteSummonLot
{
	protected readonly LockerFactory _lockerFactory;

	protected readonly LinkedContentFactory _linkedContentFactory;

	protected readonly LinkedContentAnalyticDataFactory _analyticDataFactory;

	protected readonly RouletteDropServiceFactory _rouletteDropServiceFactory;

	protected readonly SignalBus _signalBus;

	protected readonly ISaver _saver;

	protected readonly IPurchaseProcessor _purchaseProcessor;

	protected readonly IFactory<int, LinkedContentAnalyticData, LootboxLinkedContent> _lootboxContentFactory;

	protected RouletteBaseFactory(LockerFactory lockerFactory, LinkedContentFactory linkedContentFactory, LinkedContentAnalyticDataFactory analyticDataFactory, RouletteDropServiceFactory rouletteDropServiceFactory, SignalBus signalBus, ISaver saver, IPurchaseProcessor purchaseProcessor, IFactory<int, LinkedContentAnalyticData, LootboxLinkedContent> lootboxContentFactory)
	{
		_lockerFactory = lockerFactory;
		_linkedContentFactory = linkedContentFactory;
		_analyticDataFactory = analyticDataFactory;
		_rouletteDropServiceFactory = rouletteDropServiceFactory;
		_signalBus = signalBus;
		_saver = saver;
		_purchaseProcessor = purchaseProcessor;
		_lootboxContentFactory = lootboxContentFactory;
	}

	public P Create(T mapper)
	{
		ILocker locker = CreateCompositeLocker(mapper.unlock_type, mapper.unlock_value);
		(CurrencyType, CompositeIdentificator) tuple = PriceResourceHandler.ParsePriceSourse(mapper.price_resource);
		Price<int> singlePrice = new Price<int>(mapper.price, tuple.Item1, tuple.Item2);
		Price<int> wholePrice = new Price<int>(mapper.price_x10, tuple.Item1, tuple.Item2);
		LinkedContentAnalyticData linkedContentAnalyticData = _analyticDataFactory.Create(CurrencyAmplitudeAnalytic.SourceType.Roulette);
		LinkedContent defaultReward = _lootboxContentFactory.Create(mapper.reward, linkedContentAnalyticData);
		LinkedContent uniqueReward = _lootboxContentFactory.Create(mapper.main_reward, linkedContentAnalyticData);
		RouletteLot.RewardSettings rewardSettings = new RouletteLot.RewardSettings(defaultReward, uniqueReward);
		RouletteDropService rouletteDropService = _rouletteDropServiceFactory.Create(mapper.garant_id, rewardSettings);
		List<LinkedContent> list = new List<LinkedContent>();
		List<LinkedContent> list2 = new List<LinkedContent>();
		for (int i = 0; i < mapper.main_reward_type.Length; i++)
		{
			list.Add(CreateContent(mapper.main_reward_type[i], mapper.main_reward_id[i]));
		}
		for (int j = 0; j < mapper.secondary_reward_type.Length; j++)
		{
			list2.Add(CreateContent(mapper.secondary_reward_type[j], mapper.secondary_reward_id[j]));
		}
		P val = CreateRouletteLot(mapper, singlePrice, wholePrice, locker, _purchaseProcessor, rouletteDropService, list, list2, _signalBus);
		_saver.Add(val);
		return val;
	}

	protected abstract P CreateRouletteLot(T mapper, Price<int> singlePrice, Price<int> wholePrice, ILocker locker, IPurchaseProcessor purchaseProcessor, RouletteDropService rouletteDropService, List<LinkedContent> mainDropSettings, List<LinkedContent> secondaryDropSettings, SignalBus signalBus);

	protected CompositeLocker CreateCompositeLocker(UnlockType[] unlockTypes, string[] unlockValues)
	{
		ILocker[] array = new ILocker[unlockTypes.Length];
		for (int i = 0; i != unlockTypes.Length; i++)
		{
			ILocker locker = _lockerFactory.Create(unlockTypes[i], unlockValues[i], LockerSourceType.MiniEventSummon);
			array[i] = locker;
		}
		return new CompositeLocker(array);
	}

	protected LinkedContent CreateContent(RewType rewType, string reward_id)
	{
		Selector selector = SelectorTools.CreateSelector(reward_id);
		LinkedContentAnalyticData analyticData = _analyticDataFactory.Create(CurrencyAmplitudeAnalytic.SourceType.Roulette);
		return _linkedContentFactory.Create(rewType, selector, 1, 0, ContentType.Main, analyticData);
	}
}
