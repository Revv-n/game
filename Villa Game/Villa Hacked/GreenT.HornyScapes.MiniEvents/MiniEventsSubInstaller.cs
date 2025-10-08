using System.Collections.Generic;
using GreenT.HornyScapes.Bank;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.Extensions;
using StripClub.Model.Shop;
using StripClub.UI.Rewards;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.MiniEvents;

public class MiniEventsSubInstaller : MonoInstaller<MiniEventsSubInstaller>
{
	public MiniEventView MiniEventView;

	public MiniEventActivityTabView MiniEventActivityTabView;

	public MiniEventTasksRootView MiniEventTasksRootView;

	public MiniEventSingleTasksRootView MiniEventSingleTasksRootView;

	public MiniEventShopBundlesSingleRootView MiniEventShopBundlesSingleRootView;

	public MiniEventShop8SlotsSingleRootView MiniEventShop8SlotsSingleRootView;

	public MiniEventShopChainBundlesSingleRootView MiniEventShopChainBundlesSingleRootView;

	public MiniEventShopBundlesRootView MiniEventShopBundlesRootView;

	public MiniEventShop8SlotsRootView MiniEventShop8SlotsRootView;

	public MiniEventShopRouletteSummonView MiniEventShopSummonView;

	public MiniEventShopOfferView MiniEventShopOfferView;

	public MiniEventShopDoubleOfferView MiniEventShopDoubleOfferView;

	public RatingView MiniEventRatingView;

	public MiniEventViewManager MiniEventViewManager;

	public MiniEventActivityTabViewManager MiniEventActivityTabViewManager;

	public MiniEventSummonLotViewFactory MiniEventSummonLotViewFactory;

	public MiniEventSingleSummonLotViewFactory MiniEventSingleSummonLotViewFactory;

	public MiniEventRouletteSummonLotViewFactory MiniEventRouletteSummonLotViewFactory;

	public MiniEventSingleRouletteLotViewFactory MiniEventSingleRouletteLotViewFactory;

	public Transform ActivityTabsContentContainer;

	public Transform SingleActivityTabsContentContainer;

	public MiniEventsAnimationsService MiniEventsAnimationsService;

	public override void InstallBindings()
	{
		BindViewManagers();
		BindViewControllers();
		BindFactories();
		BindLocalizationResolvers();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventLotBoughtHandler>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventViewControllerService>()).AsSingle();
		((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventsAnimationsService>().FromInstance((object)MiniEventsAnimationsService).AsSingle();
		((FromBinder)((MonoInstallerBase)this).Container.Bind<BankSectionRedirectDispatcher>()).FromNew().AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<GirlPromoOpener>()).AsSingle();
	}

	private void BindViewManagers()
	{
		((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventViewManager>().FromInstance((object)MiniEventViewManager).AsSingle();
		((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventActivityTabViewManager>().FromInstance((object)MiniEventActivityTabViewManager).AsSingle();
		((FromBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventTasksRootViewManager>()).FromComponentOn(ActivityTabsContentContainer.gameObject).AsSingle();
		((FromBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventSingleTasksRootViewManager>()).FromComponentOn(SingleActivityTabsContentContainer.gameObject).AsSingle();
		((FromBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventShopBundleRootViewManager>()).FromNewComponentOn(ActivityTabsContentContainer.gameObject).AsSingle();
		((FromBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventSingleShopBundleRootViewManager>()).FromNewComponentOn(SingleActivityTabsContentContainer.gameObject).AsSingle();
		((FromBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventShopChainBundlesSingleRootViewManager>()).FromNewComponentOn(SingleActivityTabsContentContainer.gameObject).AsSingle();
		((FromBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventShop8SlotsRootViewManager>()).FromNewComponentOn(ActivityTabsContentContainer.gameObject).AsSingle();
		((FromBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventSingleShop8SlotsRootViewManager>()).FromNewComponentOn(SingleActivityTabsContentContainer.gameObject).AsSingle();
		((FromBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventShopRouletteSummonViewManager>()).FromComponentOn(ActivityTabsContentContainer.gameObject).AsSingle();
		((FromBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventShopRouletteSingleViewManager>()).FromComponentOn(SingleActivityTabsContentContainer.gameObject).AsSingle();
		((FromBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventShopSummonViewManager>()).FromComponentOn(ActivityTabsContentContainer.gameObject).AsSingle();
		((FromBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventShopSummonSingleViewManager>()).FromComponentOn(SingleActivityTabsContentContainer.gameObject).AsSingle();
		((FromBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventShopOfferViewManager>()).FromComponentOn(SingleActivityTabsContentContainer.gameObject).AsSingle();
		((FromBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventRatingViewManager>()).FromComponentOn(SingleActivityTabsContentContainer.gameObject).AsSingle();
		((FromBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventShopDoubleOfferViewManager>()).FromComponentOn(SingleActivityTabsContentContainer.gameObject).AsSingle();
	}

	private void BindViewControllers()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventViewController>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventActivityTabsViewController>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventTasksRootViewController>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventShopBundlesViewController>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventShop8SlotsViewController>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventShopSummonViewController>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventShopRouletteSummonViewController>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventShopOfferViewController>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventShopDoubleOfferViewController>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventShopChainBundlesViewController>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventShopRootViewControllerService>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventRatingRootViewController>()).AsSingle();
	}

	private void BindFactories()
	{
		((MonoInstallerBase)this).Container.BindViewFactory<MiniEvent, MiniEventView>(MiniEventViewManager.transform, MiniEventView);
		((MonoInstallerBase)this).Container.BindViewFactory<MiniEventActivityTab, MiniEventActivityTabView>(MiniEventActivityTabViewManager.transform, MiniEventActivityTabView);
		((MonoInstallerBase)this).Container.BindViewFactory<IEnumerable<MiniEventTask>, MiniEventTasksRootView>(ActivityTabsContentContainer, MiniEventTasksRootView);
		((MonoInstallerBase)this).Container.BindViewFactory<IEnumerable<MiniEventTask>, MiniEventSingleTasksRootView>(SingleActivityTabsContentContainer, MiniEventSingleTasksRootView);
		((MonoInstallerBase)this).Container.BindViewFactory<IEnumerable<Lot>, MiniEventShopBundlesRootView>(ActivityTabsContentContainer, MiniEventShopBundlesRootView);
		((MonoInstallerBase)this).Container.BindViewFactory<IEnumerable<Lot>, MiniEventShopBundlesSingleRootView>(SingleActivityTabsContentContainer, MiniEventShopBundlesSingleRootView);
		((MonoInstallerBase)this).Container.BindViewFactory<IEnumerable<Lot>, MiniEventShopChainBundlesSingleRootView>(SingleActivityTabsContentContainer, MiniEventShopChainBundlesSingleRootView);
		((MonoInstallerBase)this).Container.BindViewFactory<IEnumerable<Lot>, MiniEventShop8SlotsRootView>(ActivityTabsContentContainer, MiniEventShop8SlotsRootView);
		((MonoInstallerBase)this).Container.BindViewFactory<IEnumerable<Lot>, MiniEventShop8SlotsSingleRootView>(SingleActivityTabsContentContainer, MiniEventShop8SlotsSingleRootView);
		((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventRouletteSummonLotViewFactory>().FromInstance((object)MiniEventRouletteSummonLotViewFactory);
		((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventSingleRouletteLotViewFactory>().FromInstance((object)MiniEventSingleRouletteLotViewFactory);
		((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventSummonLotViewFactory>().FromInstance((object)MiniEventSummonLotViewFactory);
		((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventSingleSummonLotViewFactory>().FromInstance((object)MiniEventSingleSummonLotViewFactory);
		((MonoInstallerBase)this).Container.BindViewFactory<Lot, MiniEventShopOfferView>(SingleActivityTabsContentContainer, MiniEventShopOfferView);
		((MonoInstallerBase)this).Container.BindViewFactory<RatingData, RatingView>(SingleActivityTabsContentContainer, MiniEventRatingView);
		((MonoInstallerBase)this).Container.BindViewFactory<IEnumerable<Lot>, MiniEventShopDoubleOfferView>(SingleActivityTabsContentContainer, MiniEventShopDoubleOfferView);
	}

	private void BindLocalizationResolvers()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<MiniEventActivityTabsLocalizationResolver>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<MiniEventTasksDescriptionLocalizationResolver>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<MiniEventSpendCurrencyTasksLocalizationResolver>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<MiniEventAddCurrencyTasksLocalizationResolver>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<MiniEventTasksProgressLocalizationResolver>()).AsSingle();
	}
}
