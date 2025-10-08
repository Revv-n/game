using System;
using System.Runtime.CompilerServices;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Content;
using GreenT.HornyScapes.Data;
using GreenT.HornyScapes.Events.Data;
using GreenT.HornyScapes.External.StripClub._Scripts.NewEventScripts;
using GreenT.HornyScapes.Lootboxes;
using GreenT.HornyScapes.MergeCore;
using GreenT.HornyScapes.UI;
using GreenT.Settings.Data;
using GreenT.UI;
using StripClub.NewEvent.Data;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Events;

public class EventInstaller : MonoInstaller<EventInstaller>
{
	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static BindingCondition _003C_003E9__9_0;

		internal bool _003CBindFactories_003Eb__9_0(InjectContext context)
		{
			if (context.ObjectType == typeof(LinkedContentFactory))
			{
				return 1.Equals(context.ConcreteIdentifier);
			}
			return false;
		}
	}

	private const string bundleIconsSubfolderName = "MergeItemIcons";

	[SerializeField]
	private UIManager uiManager;

	public WindowGroupID EventCorePreset;

	public WindowGroupID OpenEventCorePreset;

	public WindowGroupID BattlePassCorePreset;

	public override void InstallBindings()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<EventsStateService>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesTo<EventsStateTransitionHandler>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<EventRatingFinalzer>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<EventSettingsProvider>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<BattlePassSettingsProvider>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<EventNotifyService>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<BattlePassNotifyService>()).AsSingle();
		((ArgConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<BattlePassCheckingForCompletion>()).AsSingle()).WithArguments<WindowGroupID>(BattlePassCorePreset);
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<EventProvider>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<EventStateService>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<BattlePassProvider>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<BattlePassStateService>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesTo<NotifyServiceWrapper<EventNotifyService>>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesTo<NotifyServiceWrapper<BattlePassNotifyService>>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<EventDataBuilder>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<BattlePassDataBuilder>()).AsSingle();
		((NonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<Migrate18_4To18_5>()).AsSingle()).NonLazy();
		((NonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<EventSaveMigrationFromEventIDToCalendarId>()).AsSingle()).NonLazy();
		BindSystems();
		BindCalendar();
		BindFactories();
		BindLoaders();
		BindSubsystems();
		BindLightWeightFactories();
		BindEventBattlePassModules();
		((ArgConditionCopyNonLazyBinder)((FromBinder)((MonoInstallerBase)this).Container.Bind<EventMergeScreenIndicator>()).FromNewComponentOn(uiManager.gameObject).AsSingle()).WithArguments<WindowGroupID>(OpenEventCorePreset);
		BindManagers();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MigrateVersionData>()).AsSingle();
	}

	private void BindSystems()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<EventAnalytic>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<EventStateAnalytic>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<BattlePassStateAnalytic>()).AsSingle();
	}

	private void BindSubsystems()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<TimerTracker>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<EventRewardTracker>()).AsCached();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<EventCalendarDispenser>()).AsCached();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<BattlePassCalendarDispenser>()).AsCached();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<EventLotBoughtTracker>()).AsCached();
		((NonLazyBinder)((ArgConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<EventTutorialSystem>()).AsSingle()).WithArguments<WindowGroupID>(EventCorePreset)).NonLazy();
	}

	private void BindCalendar()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<PeriodicCalendarStructureInitializer>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<PeriodicCalendarMapper>>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<PeriodicCalendarFactory>()).AsCached();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<NoviceCalendarStructureInitializer>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<NoviceCalendarMapper>>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<NoviceCalendarFactory>()).AsCached();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<EventCalendarLoader>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<BattlePassCalendarLoader>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<CalendarQueue>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<CalendarFlowRule>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<CalendarManager>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<CalendarStrategyFactory>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<EventEnergyModeTempService>()).AsSingle();
	}

	private void BindFactories()
	{
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Expected O, but got Unknown
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<EventFactory>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<BattlePassFactory>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<EventStructureInitializer>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<EventMapper>>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<BattlePassStructureInitializer>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<BattlePassMapper>>()).AsSingle();
		FromBinderNonGeneric obj = ((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<CurrencySpecialContentFactory>();
		object obj2 = _003C_003Ec._003C_003E9__9_0;
		if (obj2 == null)
		{
			BindingCondition val = (InjectContext context) => context.ObjectType == typeof(LinkedContentFactory) && 1.Equals(context.ConcreteIdentifier);
			_003C_003Ec._003C_003E9__9_0 = val;
			obj2 = (object)val;
		}
		((ConditionCopyNonLazyBinder)obj).When((BindingCondition)obj2);
		((ConditionCopyNonLazyBinder)((ConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<LinkedContentFactory>()).WithConcreteId((object)1)).WhenInjectedInto<EventFactory>();
	}

	private void BindLoaders()
	{
		((ArgConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<EventBundleDataLoader>()).AsSingle()).WithArguments<BundleType>(BundleType.Events);
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<EventShopBundleLoader>()).AsSingle();
		((ArgConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<BattlePassBundleDataLoader>()).AsSingle()).WithArguments<BundleType>(BundleType.BattlePass);
		((ArgConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<EventMergeIconsLoader>()).AsSingle()).WithArguments<BundleType, string>(BundleType.Events, "MergeItemIcons");
		((ArgConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<BattlePassMergeIconsLoader>()).AsSingle()).WithArguments<BundleType, string>(BundleType.BattlePass, "MergeItemIcons");
	}

	private void BindManagers()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<LegacyMergeIconProvider>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<BattlePassMergeIconProvider>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<EventMergeIconProvider>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<EventMapperProvider>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<BattlePassMapperProvider>()).AsSingle();
	}

	private void BindLightWeightFactories()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<EventStrategyLightWeightFactory>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<BattlePassStrategyLightWeightFactory>()).AsSingle();
	}

	private void BindEventBattlePassModules()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<EventBattlePassChecker>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<EventBattlePassLoader>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<EventBattlePassDataBuilder>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<EventBattlePassDataCleaner>()).AsSingle();
	}
}
