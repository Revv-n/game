using GreenT.HornyScapes.Bank.BankTabs;
using GreenT.HornyScapes.Bank.Data;
using GreenT.HornyScapes.Data;
using GreenT.HornyScapes.Events.Content;
using GreenT.HornyScapes.MergeStore;
using GreenT.HornyScapes.Sellouts;
using GreenT.Types;
using StripClub.Model;
using StripClub.Model.Shop;
using StripClub.Model.Shop.Data;
using Zenject;

namespace GreenT.HornyScapes.Bank;

public class BankInstaller : Installer<BankInstaller>
{
	public override void InstallBindings()
	{
		base.Container.BindInterfacesAndSelfTo<WalletProvider>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<CurrencyProcessor>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<SpendEventEnergyTracker>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<BankTabFinder>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<PurchaseProcessor>().AsSingle();
		Installer<SelloutInstaller>.Install(base.Container);
		base.Container.BindIFactory<BankTab.Manager>().FromNew();
		base.Container.BindInterfacesAndSelfTo<ContentCluster<BankTab.Manager>>().FromFactory<ContentCluster<BankTab.Manager>, ClusterFactory<BankTab.Manager, ContentCluster<BankTab.Manager>>>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<BankTabsInitializer>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<BankTabMapper>>().AsSingle();
		base.Container.BindInterfacesTo<BankTabsLoader>().AsSingle();
		base.Container.BindInterfacesTo<GreenT.HornyScapes.Bank.BankTabs.BankTabFactory>().AsCached();
		base.Container.BindIFactory<OfferSettings.Manager>().FromNew();
		base.Container.BindInterfacesAndSelfTo<OfferManagerCluster>().FromFactory<OfferManagerCluster, ClusterFactory<OfferSettings.Manager, OfferManagerCluster>>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<OfferConfigInitializer>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<OfferMapper>>().AsSingle();
		base.Container.BindInterfacesTo<OfferLoader>().AsSingle();
		base.Container.BindInterfacesTo<OfferTabFactory>().AsCached();
		base.Container.BindInterfacesAndSelfTo<LotManager>().FromNew().AsSingle();
		base.Container.BindIFactory<GoldenTicketManager>().FromNew();
		base.Container.BindInterfacesAndSelfTo<GoldenTicketManagerCluster>().FromFactory<GoldenTicketManagerCluster, ClusterFactory<GoldenTicketManager, GoldenTicketManagerCluster>>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<GoldenTicketConfigInitializer>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<GoldenTicketMapper>>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<GoldenTicketFactory>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<BankStructureInitializer>().AsSingle();
		base.Container.BindInterfacesTo<SummonLotFactory>().AsCached();
		base.Container.BindInterfacesAndSelfTo<SummonLotInitializer>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<SummonMapper>>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<SummonUseTracker>().AsSingle();
		base.Container.Bind<ResetAfterBundleLotController>().AsSingle();
		base.Container.BindInterfacesTo<ShopBundleLotFactory>().AsCached();
		base.Container.BindInterfacesAndSelfTo<BundleLotInitializer>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<ShopBundleMapper>>().AsSingle();
		base.Container.BindInterfacesTo<GemShopLotFactory>().AsCached();
		base.Container.BindInterfacesAndSelfTo<GemShopLotInitializer>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<GemShopMapper>>().AsSingle();
		base.Container.BindInterfacesTo<SubscriptionLotFactory>().AsCached();
		base.Container.BindInterfacesAndSelfTo<SubscriptionLotInitializer>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<SubscriptionLotMapper>>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<BankTabClusterCombiner>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<OfferSettingsClusterCombiner>().AsSingle();
		Installer<BannerInstaller>.Install(base.Container);
		Installer<MergeStoreInstaller>.Install(base.Container);
		base.Container.Bind<ItemFactory>().AsSingle();
		base.Container.Bind<MergeItemInfoService>().AsSingle();
		base.Container.Bind<MergeStoreService>().AsSingle().NonLazy();
		base.Container.BindInterfacesAndSelfTo<SectionCluster>().AsSingle();
		base.Container.Bind<SectionFactory>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<ItemSalesService>().AsSingle();
		base.Container.Bind<ItemsCluster>().AsSingle();
		base.Container.Bind<GreenT.HornyScapes.MergeStore.Analytics>().AsSingle();
		base.Container.Bind<SectionRefreshService>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<RouletteBankSummonLotManager>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<RouletteBankSummonFactory>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<RouletteBankSummonStructureInitializer>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<RouletteBankSummonMapper>>().AsSingle();
		base.Container.Bind<NoCurrencyTabOpener>().AsSingle();
	}
}
