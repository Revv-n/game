using GreenT.Data;
using GreenT.HornyScapes.Bank;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Data;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.Events.Data;
using GreenT.HornyScapes.External.StripClub._Scripts.MiniEvents;
using GreenT.Settings.Data;
using GreenT.UI;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.MiniEvents;

public class MiniEventsInstaller : MonoInstaller<MiniEventsInstaller>
{
	private const string bundleIconsSubfolderName = "MergeItemIcons";

	[SerializeField]
	private WindowID _startWindowID;

	public override void InstallBindings()
	{
		BindSystems();
		BindPromoPusher();
		BindCalendarModelDependencies();
		BindManagers();
		BindConfigHandlers();
		BindFactories();
		BindInitializers();
		base.Container.BindInterfacesAndSelfTo<MiniEventTimerController>().AsSingle();
	}

	private void BindSystems()
	{
		base.Container.BindInterfacesAndSelfTo<MiniEventStateAnalytic>().AsSingle();
	}

	private void BindCalendarModelDependencies()
	{
		base.Container.Bind<MiniEventsBundlesProvider>().AsSingle();
		base.Container.Bind<MiniEventsStateService>().AsSingle();
		base.Container.BindInterfacesTo<MiniEventsStateTransitionHandler>().AsSingle();
		base.Container.Bind<MiniEventMergeItemDispenser>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<MiniEventSettingsProvider>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<MiniEventDataBuilder>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<MiniEventCalendarLoader>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<MiniEventBundleDataLoader>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<MiniEventShopBundleDataLoader>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<MiniEventCalendarDispenser>().AsCached();
		base.Container.BindInterfacesAndSelfTo<MiniEventStrategyLightWeightFactory>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<MiniEventMergeIconsLoader>().AsSingle().WithArguments(BundleType.MiniEvent, "MergeItemIcons");
	}

	private void BindManagers()
	{
		base.Container.BindInterfacesAndSelfTo<MiniEventActivityTabAdministrator>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<MiniEventAdministrator>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<ActivitiesTasksManager>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<MiniEventTabsManager>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<MiniEventMapperManager>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<ActivitiesManager>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<ActivitiesQuestManager>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<ActivitiesShopManager>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<RouletteSummonLotManager>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<GarantChanceManager>().AsSingle();
		base.Container.Bind<MiniEventMergeIconsProvider>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<MiniEventMergeItemsRestoreService>().AsSingle();
		base.Container.Bind<MiniEventMergeItemsCleanService>().AsSingle();
		ConcreteIdArgConditionCopyNonLazyBinder concreteIdArgConditionCopyNonLazyBinder = base.Container.Bind<MiniEventMergeItemsDataCase>().AsSingle();
		ConcreteIdArgConditionCopyNonLazyBinder concreteIdArgConditionCopyNonLazyBinder2 = base.Container.Bind<MiniEventPocketItemsDataCase>().AsSingle();
		ConcreteIdArgConditionCopyNonLazyBinder concreteIdArgConditionCopyNonLazyBinder3 = base.Container.Bind<MiniEventInventoryItemsDataCase>().AsSingle();
		if (Application.isPlaying)
		{
			concreteIdArgConditionCopyNonLazyBinder.OnInstantiated(delegate(InjectContext _context, MiniEventMergeItemsDataCase _object)
			{
				base.Container.Resolve<ISaver>().Add(_object);
			});
			concreteIdArgConditionCopyNonLazyBinder2.OnInstantiated(delegate(InjectContext _context, MiniEventPocketItemsDataCase _object)
			{
				base.Container.Resolve<ISaver>().Add(_object);
			});
			concreteIdArgConditionCopyNonLazyBinder3.OnInstantiated(delegate(InjectContext _context, MiniEventInventoryItemsDataCase _object)
			{
				base.Container.Resolve<ISaver>().Add(_object);
			});
		}
	}

	private void BindConfigHandlers()
	{
		base.Container.BindInterfacesAndSelfTo<MiniEventActivityConfigHandler>().AsSingle().WithArguments(ConfigType.Activity);
		base.Container.BindInterfacesAndSelfTo<MiniEventRatingConfigHandler>().AsSingle().WithArguments(ConfigType.Rating);
		base.Container.BindInterfacesAndSelfTo<MiniEventConfigHandler>().AsSingle();
	}

	private void BindInitializers()
	{
		base.Container.BindInterfacesAndSelfTo<MiniEventFactory>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<MiniEventStructureInitializer>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<MiniEventMapper>>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<ActivityStructureInitializer>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<ActivityMapper>>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<ActivityQuestStructureInitializer>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<ActivityQuestMapper>>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<ActivityShopStructureInitializer>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<ActivityShopMapper>>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<GarantChanceStructureInitializer>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<GarantChanceMapper>>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<RouletteSummonStructureInitializer>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<RouletteSummonMapper>>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<MiniEventCalendarStructureInitializer>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<MiniEventCalendarMapper>>().AsSingle();
		base.Container.Bind<MiniEventCalendarFactory>().AsSingle();
	}

	private void BindFactories()
	{
		base.Container.BindInterfacesAndSelfTo<GarantChanceFactory>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<RouletteSummonFactory>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<RouletteDropServiceFactory>().AsSingle();
	}

	private void BindPromoPusher()
	{
		base.Container.BindInterfacesAndSelfTo<MiniEventsPushController>().AsCached().WithArguments(_startWindowID);
	}
}
