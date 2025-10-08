using System;
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
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventTimerController>()).AsSingle();
	}

	private void BindSystems()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventStateAnalytic>()).AsSingle();
	}

	private void BindCalendarModelDependencies()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<MiniEventsBundlesProvider>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<MiniEventsStateService>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesTo<MiniEventsStateTransitionHandler>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<MiniEventMergeItemDispenser>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventSettingsProvider>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventDataBuilder>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventCalendarLoader>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventBundleDataLoader>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventShopBundleDataLoader>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventCalendarDispenser>()).AsCached();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventStrategyLightWeightFactory>()).AsSingle();
		((ArgConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventMergeIconsLoader>()).AsSingle()).WithArguments<BundleType, string>(BundleType.MiniEvent, "MergeItemIcons");
	}

	private void BindManagers()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventActivityTabAdministrator>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventAdministrator>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<ActivitiesTasksManager>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventTabsManager>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventMapperManager>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<ActivitiesManager>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<ActivitiesQuestManager>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<ActivitiesShopManager>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<RouletteSummonLotManager>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<GarantChanceManager>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<MiniEventMergeIconsProvider>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventMergeItemsRestoreService>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<MiniEventMergeItemsCleanService>()).AsSingle();
		ConcreteIdArgConditionCopyNonLazyBinder val = ((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<MiniEventMergeItemsDataCase>()).AsSingle();
		ConcreteIdArgConditionCopyNonLazyBinder val2 = ((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<MiniEventPocketItemsDataCase>()).AsSingle();
		ConcreteIdArgConditionCopyNonLazyBinder val3 = ((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<MiniEventInventoryItemsDataCase>()).AsSingle();
		if (Application.isPlaying)
		{
			((InstantiateCallbackConditionCopyNonLazyBinder)val).OnInstantiated<MiniEventMergeItemsDataCase>((Action<InjectContext, MiniEventMergeItemsDataCase>)delegate(InjectContext _context, MiniEventMergeItemsDataCase _object)
			{
				((MonoInstallerBase)this).Container.Resolve<ISaver>().Add(_object);
			});
			((InstantiateCallbackConditionCopyNonLazyBinder)val2).OnInstantiated<MiniEventPocketItemsDataCase>((Action<InjectContext, MiniEventPocketItemsDataCase>)delegate(InjectContext _context, MiniEventPocketItemsDataCase _object)
			{
				((MonoInstallerBase)this).Container.Resolve<ISaver>().Add(_object);
			});
			((InstantiateCallbackConditionCopyNonLazyBinder)val3).OnInstantiated<MiniEventInventoryItemsDataCase>((Action<InjectContext, MiniEventInventoryItemsDataCase>)delegate(InjectContext _context, MiniEventInventoryItemsDataCase _object)
			{
				((MonoInstallerBase)this).Container.Resolve<ISaver>().Add(_object);
			});
		}
	}

	private void BindConfigHandlers()
	{
		((ArgConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventActivityConfigHandler>()).AsSingle()).WithArguments<ConfigType>(ConfigType.Activity);
		((ArgConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventRatingConfigHandler>()).AsSingle()).WithArguments<ConfigType>(ConfigType.Rating);
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventConfigHandler>()).AsSingle();
	}

	private void BindInitializers()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventFactory>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventStructureInitializer>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<MiniEventMapper>>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<ActivityStructureInitializer>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<ActivityMapper>>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<ActivityQuestStructureInitializer>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<ActivityQuestMapper>>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<ActivityShopStructureInitializer>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<ActivityShopMapper>>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<GarantChanceStructureInitializer>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<GarantChanceMapper>>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<RouletteSummonStructureInitializer>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<RouletteSummonMapper>>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventCalendarStructureInitializer>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<MiniEventCalendarMapper>>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<MiniEventCalendarFactory>()).AsSingle();
	}

	private void BindFactories()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<GarantChanceFactory>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<RouletteSummonFactory>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<RouletteDropServiceFactory>()).AsSingle();
	}

	private void BindPromoPusher()
	{
		((ArgConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventsPushController>()).AsCached()).WithArguments<WindowID>(_startWindowID);
	}
}
