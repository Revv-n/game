using System;
using StripClub.Model.Shop;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.MergeStore;

public class SectionRefreshService
{
	private readonly ICurrencyProcessor _currencyProcessor;

	private readonly ItemFactory _itemFactory;

	private readonly Analytics _analytics;

	private readonly ItemsCluster _assortment;

	private readonly SignalBus _signalBus;

	public SectionRefreshService(ICurrencyProcessor currencyProcessor, ItemsCluster assortment, ItemFactory itemFactory, Analytics analytics, SignalBus signalBus)
	{
		_currencyProcessor = currencyProcessor;
		_assortment = assortment;
		_itemFactory = itemFactory;
		_analytics = analytics;
		_signalBus = signalBus;
	}

	public void CreatStreams(string bundle, StoreSection section)
	{
		IDisposable requestRefreshStream = ObservableExtensions.Subscribe<SectionRefreshRequest>(section.OnRequestRefresh, (Action<SectionRefreshRequest>)delegate(SectionRefreshRequest request)
		{
			ProcessRefreshSectionRequest(request, bundle);
		});
		ObservableExtensions.Subscribe<StoreSection>(Observable.Take<StoreSection>(section.OnClear, 1), (Action<StoreSection>)delegate
		{
			requestRefreshStream.Dispose();
		});
	}

	private void ProcessRefreshSectionRequest(SectionRefreshRequest request, string bundle)
	{
		if (request.Cost.Amount <= 0 || _currencyProcessor.TrySpent(request.Cost))
		{
			_analytics.TrackRefresh(request.Cost, request.Section);
			_signalBus.TrySendSpendHardMergeStoreSignal(request.Cost);
			PopulateWithNewItems(request.Section, bundle);
		}
	}

	public StoreSection PopulateWithNewItems(StoreSection section, string bundle)
	{
		Item[] array = _itemFactory.GenerateItemsForSection(section, bundle);
		section.Refresh(array);
		_assortment.UpdateAssortment(array, section);
		return section;
	}

	public StoreSection InitializationWithOldItems(StoreSection section)
	{
		_assortment.UpdateAssortment(section.GetItems(), section);
		section.Initialization();
		return section;
	}
}
