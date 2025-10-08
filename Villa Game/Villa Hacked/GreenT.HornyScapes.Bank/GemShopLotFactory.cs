using System;
using GreenT.Data;
using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.Lootboxes;
using GreenT.HornyScapes.Monetization;
using GreenT.HornyScapes.Monetization.Webgl;
using GreenT.Types;
using StripClub.Model;
using StripClub.Model.Shop;
using StripClub.Model.Shop.Data;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Bank;

public class GemShopLotFactory : LotFactory<GemShopMapper, GemShopLot>
{
	private readonly LinkedContentAnalyticDataFactory analyticDataFactory;

	private readonly BundlesProviderBase bundlesProvider;

	private readonly LinkedContentFactory linkedContentFactory;

	private readonly PriceChecker _priceChecker;

	private readonly IRegionPriceResolver _priceResolver;

	public GemShopLotFactory(LockerFactory lockerFactory, ISaver saver, IPurchaseProcessor purchaseProcessor, IRegionPriceResolver priceResolver, BundlesProviderBase bundlesProvider, LinkedContentAnalyticDataFactory analyticDataFactory, LinkedContentFactory linkedContentFactory, [InjectOptional] PriceChecker priceChecker)
		: base(lockerFactory, saver, purchaseProcessor)
	{
		this.bundlesProvider = bundlesProvider;
		_priceResolver = priceResolver;
		this.analyticDataFactory = analyticDataFactory;
		this.linkedContentFactory = linkedContentFactory;
		_priceChecker = priceChecker;
	}

	public override GemShopLot Create(GemShopMapper mapper)
	{
		EqualityLocker<int> countLocker;
		ILocker locker = CreateCompositeLocker(mapper, out countLocker, LockerSourceType.GemShopLot);
		try
		{
			LinkedContent reward = CreateContent(mapper.reward_id, mapper.reward_count, mapper.reward_type);
			LinkedContent extraReward = null;
			if (mapper.extra_reward_count.HasValue)
			{
				Enum.TryParse<RewType>(mapper.extra_reward_type, ignoreCase: true, out var result);
				extraReward = CreateContent(mapper.extra_reward_id, mapper.extra_reward_count.Value, result);
			}
			MonetizationRequestData data = new MonetizationRequestData(mapper.item_name, mapper.item_descr, mapper.image_name);
			(CurrencyType, CompositeIdentificator) tuple = PriceResourceHandler.ParsePriceSourse(mapper.price_resource);
			decimal price = ((tuple.Item1 == CurrencyType.Real) ? _priceResolver.Prices[mapper.lot_id] : mapper.price);
			GemShopLot gemShopLot = new GemShopLot(mapper, bundlesProvider, mapper.content_source, reward, extraReward, locker, countLocker, price, purchaseProcessor, data, tuple.Item1, tuple.Item2);
			saver.Add(gemShopLot);
			return gemShopLot;
		}
		catch (Exception ex)
		{
			Debug.LogError($"Captured error while creating gemshop lot with id {mapper.id} {mapper.price_resource} {mapper.lot_id}: " + ex.Message);
			return null;
		}
	}

	internal LinkedContent CreateContent(string reward_id, int reward_count, RewType rewType)
	{
		Selector selector = SelectorTools.CreateSelector(reward_id);
		LinkedContentAnalyticData analyticData = analyticDataFactory.Create(CurrencyAmplitudeAnalytic.SourceType.Bought);
		return linkedContentFactory.Create(rewType, selector, reward_count, 0, ContentType.Main, analyticData);
	}
}
