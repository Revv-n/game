using System.Collections.Generic;
using GreenT.Data;
using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.Lootboxes;
using GreenT.HornyScapes.MiniEvents;
using StripClub.Model;
using StripClub.Model.Shop;
using Zenject;

namespace GreenT.HornyScapes.Bank;

public sealed class RouletteSummonFactory : RouletteBaseFactory<RouletteSummonMapper, RouletteSummonLot>
{
	public RouletteSummonFactory(LockerFactory lockerFactory, LinkedContentFactory linkedContentFactory, LinkedContentAnalyticDataFactory analyticDataFactory, RouletteDropServiceFactory rouletteDropServiceFactory, SignalBus signalBus, ISaver saver, IPurchaseProcessor purchaseProcessor, IFactory<int, LinkedContentAnalyticData, LootboxLinkedContent> lootboxContentFactory)
		: base(lockerFactory, linkedContentFactory, analyticDataFactory, rouletteDropServiceFactory, signalBus, saver, purchaseProcessor, lootboxContentFactory)
	{
	}

	protected override RouletteSummonLot CreateRouletteLot(RouletteSummonMapper mapper, Price<int> singlePrice, Price<int> wholesalePrice, ILocker locker, IPurchaseProcessor purchaseProcessor, RouletteDropService rouletteDropService, List<LinkedContent> mainDropSettings, List<LinkedContent> secondaryDropSettings, SignalBus signalBus)
	{
		return new RouletteSummonLot(mapper, singlePrice, wholesalePrice, locker, _purchaseProcessor, rouletteDropService, mainDropSettings, secondaryDropSettings, _signalBus);
	}
}
