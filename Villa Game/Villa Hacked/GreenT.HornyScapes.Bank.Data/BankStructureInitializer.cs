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

	public IReadOnlyReactiveProperty<bool> IsInitialized => (IReadOnlyReactiveProperty<bool>)(object)isInitialized;

	public IReadOnlyReactiveProperty<bool> IsRequiredInitialized => (IReadOnlyReactiveProperty<bool>)(object)isRequiredInitialized;

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
		isInitialized = ReactivePropertyExtensions.ToReadOnlyReactiveProperty<bool>(Observable.Select<IList<bool>, bool>(Observable.CombineLatest<bool>((IEnumerable<IObservable<bool>>)initializers.Select((IInitializerState _initializer) => _initializer.IsInitialized)), (Func<IList<bool>, bool>)((IList<bool> _initStates) => _initStates.All((bool _isInited) => _isInited))), false);
		bool flag = initializers.All((IInitializerState _initializer) => _initializer.IsRequiredInitialized.Value);
		isRequiredInitialized = ReactivePropertyExtensions.ToReadOnlyReactiveProperty<bool>(Observable.Select<IList<bool>, bool>(Observable.CombineLatest<bool>((IEnumerable<IObservable<bool>>)initializers.Select((IInitializerState _initializer) => _initializer.IsRequiredInitialized)), (Func<IList<bool>, bool>)((IList<bool> _requiredStates) => _requiredStates.All((bool _isInited) => _isInited))), flag);
	}

	public IObservable<bool> Initialize(ConfigParser.Folder configStructure)
	{
		IObservable<OfferMapper[]> observable = Observable.Share<OfferMapper[]>(Observable.SubscribeOnMainThread<OfferMapper[]>(Observable.Select<bool, OfferMapper[]>(Observable.Where<bool>(Observable.ContinueWith<bool, bool>(Observable.Where<bool>(Observable.ContinueWith<bool, bool>(Observable.Where<bool>(Observable.ContinueWith<bool, bool>(Observable.Where<bool>(Observable.ContinueWith<bool, bool>(Observable.Where<bool>(Observable.ContinueWith<bool, bool>(Observable.Where<bool>(Observable.ContinueWith<bool, bool>(Observable.Where<bool>(bankTabsInitializer.Initialize(configStructure, RequestType.ShopCategories), (Func<bool, bool>)((bool _isInited) => _isInited)), (Func<bool, IObservable<bool>>)((bool _) => gemShopLotInitializer.Initialize(configStructure, RequestType.GemShop))), (Func<bool, bool>)((bool _isInited) => _isInited)), (Func<bool, IObservable<bool>>)((bool _) => summonLotInitializer.Initialize(configStructure, RequestType.SummonShop))), (Func<bool, bool>)((bool _isInited) => _isInited)), (Func<bool, IObservable<bool>>)((bool _) => bundleLotInitializer.Initialize(configStructure, RequestType.BundlesShop))), (Func<bool, bool>)((bool _isInited) => _isInited)), (Func<bool, IObservable<bool>>)((bool _) => goldenTicketInitializer.Initialize(configStructure, RequestType.GoldenTicket))), (Func<bool, bool>)((bool _isInited) => _isInited)), (Func<bool, IObservable<bool>>)((bool _) => _subscriptionLotInitializer.Initialize(configStructure, RequestType.Subscription))), (Func<bool, bool>)((bool _isInited) => _isInited)), (Func<bool, IObservable<bool>>)((bool _) => _subscriptionOfferInitializer.Initialize(configStructure, RequestType.SubscriptionPreviewSettings))), (Func<bool, bool>)((bool _isInited) => _isInited)), (Func<bool, OfferMapper[]>)((bool _) => configStructure.GetContentArray<OfferMapper>(RequestType.OfferSettings, string.Empty)))));
		IObservable<bool> observable2 = Observable.Empty<bool>();
		foreach (IStructureInitializer<IEnumerable<OfferMapper>> offersConfigInitializer in offersConfigInitializerList)
		{
			IObservable<bool> observable3 = Observable.Where<bool>(Observable.ContinueWith<OfferMapper[], bool>(observable, (Func<OfferMapper[], IObservable<bool>>)offersConfigInitializer.Initialize), (Func<bool, bool>)((bool _isInited) => _isInited));
			observable2 = Observable.Merge<bool>(observable2, new IObservable<bool>[1] { observable3 });
		}
		return observable2.Debug("Bank: Load Shop", LogType.Data);
	}

	public void Dispose()
	{
		isInitialized.Dispose();
		isRequiredInitialized.Dispose();
	}
}
