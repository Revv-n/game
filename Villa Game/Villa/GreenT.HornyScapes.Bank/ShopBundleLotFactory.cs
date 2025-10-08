using System;
using GreenT.AssetBundles;
using GreenT.Data;
using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.Monetization;
using GreenT.HornyScapes.Monetization.Webgl;
using GreenT.Types;
using StripClub.Model;
using StripClub.Model.Shop;
using StripClub.Model.Shop.Data;
using Zenject;

namespace GreenT.HornyScapes.Bank;

public class ShopBundleLotFactory : LotFactory<ShopBundleMapper, BundleLot>
{
	private readonly ResetAfterBundleLotController _resetAfterBundleLotController;

	private readonly IClock clock;

	private readonly IRegionPriceResolver _priceResolver;

	private readonly LinkedContentAnalyticDataFactory analyticDataFactory;

	private readonly IFactory<int, LinkedContentAnalyticData, LootboxLinkedContent> lootboxContentFactory;

	private readonly AssetProvider _assetProvider;

	private readonly BundlesProviderBase _bundlesProvider;

	public ShopBundleLotFactory(ResetAfterBundleLotController resetAfterBundleLotController, LockerFactory lockerFactory, ISaver saver, IPurchaseProcessor purchaseProcessor, IClock clock, LinkedContentAnalyticDataFactory analyticDataFactory, IFactory<int, LinkedContentAnalyticData, LootboxLinkedContent> lootboxContentFactory, IRegionPriceResolver priceResolver, AssetProvider assetProvider, BundlesProviderBase bundlesProvider)
		: base(lockerFactory, saver, purchaseProcessor)
	{
		_resetAfterBundleLotController = resetAfterBundleLotController;
		_priceResolver = priceResolver;
		this.clock = clock;
		this.analyticDataFactory = analyticDataFactory;
		this.lootboxContentFactory = lootboxContentFactory;
		_assetProvider = assetProvider;
		_bundlesProvider = bundlesProvider;
	}

	public override BundleLot Create(ShopBundleMapper mapper)
	{
		try
		{
			EqualityLocker<int> countLocker;
			ILocker locker = CreateCompositeLocker(mapper, out countLocker, LockerSourceType.BundleLot);
			LinkedContentAnalyticData param = analyticDataFactory.Create(CurrencyAmplitudeAnalytic.SourceType.Bought);
			LinkedContent reward = lootboxContentFactory.Create(mapper.lootbox_id, param);
			BundleLot.ViewSettings viewSettings = new BundleLot.ViewSettings(_assetProvider, _bundlesProvider, mapper.content_source, mapper.view_prefab, mapper.view_parameters);
			MonetizationRequestData data = new MonetizationRequestData(mapper.item_name, mapper.item_descr, mapper.image_name);
			(CurrencyType, CompositeIdentificator) tuple = PriceResourceHandler.ParsePriceSourse(mapper.price_resource);
			decimal price = ((tuple.Item1 == CurrencyType.Real) ? _priceResolver.Prices[mapper.lot_id] : mapper.price);
			BundleLot bundleLot = new BundleLot(mapper, viewSettings, reward, locker, countLocker, purchaseProcessor, price, clock.GetTime, data, tuple.Item1, tuple.Item2);
			_resetAfterBundleLotController.Add(mapper.reset_after, bundleLot);
			saver.Add(bundleLot);
			return bundleLot;
		}
		catch (Exception innerException)
		{
			throw innerException.SendException(GetType().Name + ": Error when creating shop bundle id:" + mapper.id + " Check config table \"BankItems\" => \"Bundles\" ");
		}
	}
}
