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
		base.Container.BindInterfacesAndSelfTo<MiniEventLotBoughtHandler>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<MiniEventViewControllerService>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<MiniEventsAnimationsService>().FromInstance(MiniEventsAnimationsService).AsSingle();
		base.Container.Bind<BankSectionRedirectDispatcher>().FromNew().AsSingle();
		base.Container.Bind<GirlPromoOpener>().AsSingle();
	}

	private void BindViewManagers()
	{
		base.Container.BindInterfacesAndSelfTo<MiniEventViewManager>().FromInstance(MiniEventViewManager).AsSingle();
		base.Container.BindInterfacesAndSelfTo<MiniEventActivityTabViewManager>().FromInstance(MiniEventActivityTabViewManager).AsSingle();
		base.Container.BindInterfacesAndSelfTo<MiniEventTasksRootViewManager>().FromComponentOn(ActivityTabsContentContainer.gameObject).AsSingle();
		base.Container.BindInterfacesAndSelfTo<MiniEventSingleTasksRootViewManager>().FromComponentOn(SingleActivityTabsContentContainer.gameObject).AsSingle();
		base.Container.BindInterfacesAndSelfTo<MiniEventShopBundleRootViewManager>().FromNewComponentOn(ActivityTabsContentContainer.gameObject).AsSingle();
		base.Container.BindInterfacesAndSelfTo<MiniEventSingleShopBundleRootViewManager>().FromNewComponentOn(SingleActivityTabsContentContainer.gameObject).AsSingle();
		base.Container.BindInterfacesAndSelfTo<MiniEventShopChainBundlesSingleRootViewManager>().FromNewComponentOn(SingleActivityTabsContentContainer.gameObject).AsSingle();
		base.Container.BindInterfacesAndSelfTo<MiniEventShop8SlotsRootViewManager>().FromNewComponentOn(ActivityTabsContentContainer.gameObject).AsSingle();
		base.Container.BindInterfacesAndSelfTo<MiniEventSingleShop8SlotsRootViewManager>().FromNewComponentOn(SingleActivityTabsContentContainer.gameObject).AsSingle();
		base.Container.BindInterfacesAndSelfTo<MiniEventShopRouletteSummonViewManager>().FromComponentOn(ActivityTabsContentContainer.gameObject).AsSingle();
		base.Container.BindInterfacesAndSelfTo<MiniEventShopRouletteSingleViewManager>().FromComponentOn(SingleActivityTabsContentContainer.gameObject).AsSingle();
		base.Container.BindInterfacesAndSelfTo<MiniEventShopSummonViewManager>().FromComponentOn(ActivityTabsContentContainer.gameObject).AsSingle();
		base.Container.BindInterfacesAndSelfTo<MiniEventShopSummonSingleViewManager>().FromComponentOn(SingleActivityTabsContentContainer.gameObject).AsSingle();
		base.Container.BindInterfacesAndSelfTo<MiniEventShopOfferViewManager>().FromComponentOn(SingleActivityTabsContentContainer.gameObject).AsSingle();
		base.Container.BindInterfacesAndSelfTo<MiniEventRatingViewManager>().FromComponentOn(SingleActivityTabsContentContainer.gameObject).AsSingle();
		base.Container.BindInterfacesAndSelfTo<MiniEventShopDoubleOfferViewManager>().FromComponentOn(SingleActivityTabsContentContainer.gameObject).AsSingle();
	}

	private void BindViewControllers()
	{
		base.Container.BindInterfacesAndSelfTo<MiniEventViewController>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<MiniEventActivityTabsViewController>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<MiniEventTasksRootViewController>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<MiniEventShopBundlesViewController>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<MiniEventShop8SlotsViewController>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<MiniEventShopSummonViewController>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<MiniEventShopRouletteSummonViewController>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<MiniEventShopOfferViewController>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<MiniEventShopDoubleOfferViewController>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<MiniEventShopChainBundlesViewController>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<MiniEventShopRootViewControllerService>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<MiniEventRatingRootViewController>().AsSingle();
	}

	private void BindFactories()
	{
		base.Container.BindViewFactory<MiniEvent, MiniEventView>(MiniEventViewManager.transform, MiniEventView);
		base.Container.BindViewFactory<MiniEventActivityTab, MiniEventActivityTabView>(MiniEventActivityTabViewManager.transform, MiniEventActivityTabView);
		base.Container.BindViewFactory<IEnumerable<MiniEventTask>, MiniEventTasksRootView>(ActivityTabsContentContainer, MiniEventTasksRootView);
		base.Container.BindViewFactory<IEnumerable<MiniEventTask>, MiniEventSingleTasksRootView>(SingleActivityTabsContentContainer, MiniEventSingleTasksRootView);
		base.Container.BindViewFactory<IEnumerable<Lot>, MiniEventShopBundlesRootView>(ActivityTabsContentContainer, MiniEventShopBundlesRootView);
		base.Container.BindViewFactory<IEnumerable<Lot>, MiniEventShopBundlesSingleRootView>(SingleActivityTabsContentContainer, MiniEventShopBundlesSingleRootView);
		base.Container.BindViewFactory<IEnumerable<Lot>, MiniEventShopChainBundlesSingleRootView>(SingleActivityTabsContentContainer, MiniEventShopChainBundlesSingleRootView);
		base.Container.BindViewFactory<IEnumerable<Lot>, MiniEventShop8SlotsRootView>(ActivityTabsContentContainer, MiniEventShop8SlotsRootView);
		base.Container.BindViewFactory<IEnumerable<Lot>, MiniEventShop8SlotsSingleRootView>(SingleActivityTabsContentContainer, MiniEventShop8SlotsSingleRootView);
		base.Container.BindInterfacesAndSelfTo<MiniEventRouletteSummonLotViewFactory>().FromInstance(MiniEventRouletteSummonLotViewFactory);
		base.Container.BindInterfacesAndSelfTo<MiniEventSingleRouletteLotViewFactory>().FromInstance(MiniEventSingleRouletteLotViewFactory);
		base.Container.BindInterfacesAndSelfTo<MiniEventSummonLotViewFactory>().FromInstance(MiniEventSummonLotViewFactory);
		base.Container.BindInterfacesAndSelfTo<MiniEventSingleSummonLotViewFactory>().FromInstance(MiniEventSingleSummonLotViewFactory);
		base.Container.BindViewFactory<Lot, MiniEventShopOfferView>(SingleActivityTabsContentContainer, MiniEventShopOfferView);
		base.Container.BindViewFactory<RatingData, RatingView>(SingleActivityTabsContentContainer, MiniEventRatingView);
		base.Container.BindViewFactory<IEnumerable<Lot>, MiniEventShopDoubleOfferView>(SingleActivityTabsContentContainer, MiniEventShopDoubleOfferView);
	}

	private void BindLocalizationResolvers()
	{
		base.Container.Bind<MiniEventActivityTabsLocalizationResolver>().AsSingle();
		base.Container.Bind<MiniEventTasksDescriptionLocalizationResolver>().AsSingle();
		base.Container.Bind<MiniEventSpendCurrencyTasksLocalizationResolver>().AsSingle();
		base.Container.Bind<MiniEventAddCurrencyTasksLocalizationResolver>().AsSingle();
		base.Container.Bind<MiniEventTasksProgressLocalizationResolver>().AsSingle();
	}
}
