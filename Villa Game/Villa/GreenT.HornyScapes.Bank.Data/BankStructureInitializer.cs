using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Bank.BankTabs;
using GreenT.HornyScapes.Data;
using GreenT.HornyScapes.Subscription.Push;
using GreenT.Settings.Data;
using StripClub.Model.Data;
using StripClub.Model.Shop;
using StripClub.Model.Shop.Data;
using UniRx;

namespace GreenT.HornyScapes.Bank.Data;

public class BankStructureInitializer : IStructureInitializer<ConfigParser.Folder>, IInitializerState, IDisposable
{
	private IStructureInitializer<ConfigParser.Folder, RequestType> bankTabsInitializer;

	private IStructureInitializer<ConfigParser.Folder, RequestType> gemShopLotInitializer;

	private IStructureInitializer<ConfigParser.Folder, RequestType> summonLotInitializer;

	private IStructureInitializer<ConfigParser.Folder, RequestType> bundleLotInitializer;

	private IStructureInitializer<ConfigParser.Folder, RequestType> goldenTicketInitializer;

	private IStructureInitializer<ConfigParser.Folder, RequestType> _subscriptionLotInitializer;

	private List<IStructureInitializer<IEnumerable<OfferMapper>>> offersConfigInitializerList;

	private readonly StructureInitializerProxyWithArrayFromConfig<SubscriptionPushMapper> _subscriptionOfferInitializer;

	private List<IInitializerState> initializers;

	private readonly ReadOnlyReactiveProperty<bool> isInitialized;

	protected readonly ReadOnlyReactiveProperty<bool> isRequiredInitialized;

	public IReadOnlyReactiveProperty<bool> IsInitialized => isInitialized;

	public IReadOnlyReactiveProperty<bool> IsRequiredInitialized => isRequiredInitialized;

	public BankStructureInitializer(StructureInitializerProxyWithArrayFromConfig<BankTabMapper> bankTabsInitializer, StructureInitializerProxyWithArrayFromConfig<GemShopMapper> gemShopLotInitializer, StructureInitializerProxyWithArrayFromConfig<SummonMapper> summonLotInitializer, StructureInitializerProxyWithArrayFromConfig<ShopBundleMapper> bundleLotInitializer, StructureInitializerProxyWithArrayFromConfig<SubscriptionPushMapper> subscriptionOfferInitializer, StructureInitializerProxyWithArrayFromConfig<GoldenTicketMapper> goldenTicketConfigInitializerList, StructureInitializerProxyWithArrayFromConfig<SubscriptionLotMapper> subscriptionLotInitializer, List<IStructureInitializer<IEnumerable<OfferMapper>>> offersConfigInitializerList)
	{
		this.bankTabsInitializer = bankTabsInitializer;
		this.gemShopLotInitializer = gemShopLotInitializer;
		this.summonLotInitializer = summonLotInitializer;
		this.bundleLotInitializer = bundleLotInitializer;
		this.offersConfigInitializerList = offersConfigInitializerList;
		goldenTicketInitializer = goldenTicketConfigInitializerList;
		_subscriptionLotInitializer = subscriptionLotInitializer;
		_subscriptionOfferInitializer = subscriptionOfferInitializer;
		initializers = new List<IInitializerState> { this.bankTabsInitializer, this.gemShopLotInitializer, this.summonLotInitializer, this.bundleLotInitializer, goldenTicketInitializer, _subscriptionLotInitializer };
		initializers.AddRange(offersConfigInitializerList);
		isInitialized = (from _initStates in initializers.Select((IInitializerState _initializer) => _initializer.IsInitialized).CombineLatest()
			select _initStates.All((bool _isInited) => _isInited)).ToReadOnlyReactiveProperty(initialValue: false);
		bool initialValue = initializers.All((IInitializerState _initializer) => _initializer.IsRequiredInitialized.Value);
		isRequiredInitialized = (from _requiredStates in initializers.Select((IInitializerState _initializer) => _initializer.IsRequiredInitialized).CombineLatest()
			select _requiredStates.All((bool _isInited) => _isInited)).ToReadOnlyReactiveProperty(initialValue);
	}

	public IObservable<bool> Initialize(ConfigParser.Folder configStructure)
	{
		IObservable<OfferMapper[]> source = (from _isInited in (from _isInited in (from _isInited in (from _isInited in (from _isInited in (from _isInited in (from _isInited in bankTabsInitializer.Initialize(configStructure, RequestType.ShopCategories)
									where _isInited
									select _isInited).ContinueWith((bool _) => gemShopLotInitializer.Initialize(configStructure, RequestType.GemShop))
								where _isInited
								select _isInited).ContinueWith((bool _) => summonLotInitializer.Initialize(configStructure, RequestType.SummonShop))
							where _isInited
							select _isInited).ContinueWith((bool _) => bundleLotInitializer.Initialize(configStructure, RequestType.BundlesShop))
						where _isInited
						select _isInited).ContinueWith((bool _) => goldenTicketInitializer.Initialize(configStructure, RequestType.GoldenTicket))
					where _isInited
					select _isInited).ContinueWith((bool _) => _subscriptionLotInitializer.Initialize(configStructure, RequestType.Subscription))
				where _isInited
				select _isInited).ContinueWith((bool _) => _subscriptionOfferInitializer.Initialize(configStructure, RequestType.SubscriptionPreviewSettings))
			where _isInited
			select _isInited into _
			select configStructure.GetContentArray<OfferMapper>(RequestType.OfferSettings, string.Empty)).SubscribeOnMainThread().Share();
		IObservable<bool> observable = Observable.Empty<bool>();
		foreach (IStructureInitializer<IEnumerable<OfferMapper>> offersConfigInitializer in offersConfigInitializerList)
		{
			IObservable<bool> observable2 = from _isInited in source.ContinueWith(offersConfigInitializer.Initialize)
				where _isInited
				select _isInited;
			observable = observable.Merge(observable2);
		}
		return observable.Debug("Bank: Load Shop", LogType.Data);
	}

	public void Dispose()
	{
		isInitialized.Dispose();
		isRequiredInitialized.Dispose();
	}
}
