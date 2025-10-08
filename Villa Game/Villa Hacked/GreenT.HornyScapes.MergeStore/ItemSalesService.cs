using System;
using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.Content;
using GreenT.HornyScapes.Lootboxes;
using StripClub.Model;
using StripClub.Model.Data;
using StripClub.Model.Shop;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.MergeStore;

public class ItemSalesService : IInitializable, IDisposable
{
	private readonly ICurrencyProcessor _currencyProcessor;

	private readonly IContentAdder _contentAdder;

	private readonly MergeItemInfoService _mergeItemInfoService;

	private readonly LinkedContentAnalyticDataFactory _analyticDataFactory;

	private readonly LinkedContentFactory _linkedFactory;

	private readonly ItemsCluster _itemsCluster;

	private readonly Analytics _analytics;

	private readonly SignalBus _signalBus;

	private IDisposable _purchasedStream;

	public ItemSalesService(ICurrencyProcessor currencyProcessor, IContentAdder contentAdder, MergeItemInfoService mergeItemInfoService, LinkedContentAnalyticDataFactory analyticDataFactory, LinkedContentFactory linkedFactory, ItemsCluster itemsCluster, Analytics analytics, SignalBus signalBus)
	{
		_currencyProcessor = currencyProcessor;
		_contentAdder = contentAdder;
		_mergeItemInfoService = mergeItemInfoService;
		_analyticDataFactory = analyticDataFactory;
		_linkedFactory = linkedFactory;
		_itemsCluster = itemsCluster;
		_analytics = analytics;
		_signalBus = signalBus;
	}

	private void TryPurchased(ItemBuyRequest request)
	{
		Cost cost = new Cost(request.Item.SalePrice, request.Item.CurrencyType);
		if (_currencyProcessor.TrySpent(cost))
		{
			_analytics.BoughtItem(cost, request);
			_signalBus.TrySendSpendHardMergeStoreSignal(cost);
			_signalBus.SendBuyItemInMergeStoreSignal();
			request.Item.Buy();
			LinkedContent linkedContent = GetLinkedContent(request.Item);
			_contentAdder.AddContent(linkedContent);
		}
	}

	private LinkedContent GetLinkedContent(Item item)
	{
		Selector selector = SelectorTools.CreateSelector(_mergeItemInfoService.GetConfig(item.ItemKey).UniqId.ToString());
		LinkedContentAnalyticData analyticData = _analyticDataFactory.Create(CurrencyAmplitudeAnalytic.SourceType.MergeShop);
		return _linkedFactory.Create(RewType.MergeItem, selector, item.Amount, 0, item.ContentType, analyticData);
	}

	public void Initialize()
	{
		_purchasedStream = ObservableExtensions.Subscribe<ItemBuyRequest>(_itemsCluster.OnPurchasedRequest, (Action<ItemBuyRequest>)TryPurchased);
	}

	public void Dispose()
	{
		_purchasedStream.Dispose();
	}
}
