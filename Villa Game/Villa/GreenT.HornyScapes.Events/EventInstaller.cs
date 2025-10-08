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
	private const string bundleIconsSubfolderName = "MergeItemIcons";

	[SerializeField]
	private UIManager uiManager;

	public WindowGroupID EventCorePreset;

	public WindowGroupID OpenEventCorePreset;

	public WindowGroupID BattlePassCorePreset;

	public override void InstallBindings()
	{
		base.Container.Bind<EventsStateService>().AsSingle();
		base.Container.BindInterfacesTo<EventsStateTransitionHandler>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<EventRatingFinalzer>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<EventSettingsProvider>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<BattlePassSettingsProvider>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<EventNotifyService>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<BattlePassNotifyService>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<BattlePassCheckingForCompletion>().AsSingle().WithArguments(BattlePassCorePreset);
		base.Container.Bind<EventProvider>().AsSingle();
		base.Container.Bind<EventStateService>().AsSingle();
		base.Container.Bind<BattlePassProvider>().AsSingle();
		base.Container.Bind<BattlePassStateService>().AsSingle();
		base.Container.BindInterfacesTo<NotifyServiceWrapper<EventNotifyService>>().AsSingle();
		base.Container.BindInterfacesTo<NotifyServiceWrapper<BattlePassNotifyService>>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<EventDataBuilder>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<BattlePassDataBuilder>().AsSingle();
		base.Container.Bind<Migrate18_4To18_5>().AsSingle().NonLazy();
		base.Container.BindInterfacesAndSelfTo<EventSaveMigrationFromEventIDToCalendarId>().AsSingle().NonLazy();
		BindSystems();
		BindCalendar();
		BindFactories();
		BindLoaders();
		BindSubsystems();
		BindLightWeightFactories();
		BindEventBattlePassModules();
		base.Container.Bind<EventMergeScreenIndicator>().FromNewComponentOn(uiManager.gameObject).AsSingle()
			.WithArguments(OpenEventCorePreset);
		BindManagers();
		base.Container.BindInterfacesAndSelfTo<MigrateVersionData>().AsSingle();
	}

	private void BindSystems()
	{
		base.Container.Bind<EventAnalytic>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<EventStateAnalytic>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<BattlePassStateAnalytic>().AsSingle();
	}

	private void BindSubsystems()
	{
		base.Container.BindInterfacesAndSelfTo<TimerTracker>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<EventRewardTracker>().AsCached();
		base.Container.BindInterfacesAndSelfTo<EventCalendarDispenser>().AsCached();
		base.Container.BindInterfacesAndSelfTo<BattlePassCalendarDispenser>().AsCached();
		base.Container.BindInterfacesAndSelfTo<EventLotBoughtTracker>().AsCached();
		base.Container.BindInterfacesAndSelfTo<EventTutorialSystem>().AsSingle().WithArguments(EventCorePreset)
			.NonLazy();
	}

	private void BindCalendar()
	{
		base.Container.BindInterfacesAndSelfTo<PeriodicCalendarStructureInitializer>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<PeriodicCalendarMapper>>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<PeriodicCalendarFactory>().AsCached();
		base.Container.BindInterfacesAndSelfTo<NoviceCalendarStructureInitializer>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<NoviceCalendarMapper>>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<NoviceCalendarFactory>().AsCached();
		base.Container.BindInterfacesAndSelfTo<EventCalendarLoader>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<BattlePassCalendarLoader>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<CalendarQueue>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<CalendarFlowRule>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<CalendarManager>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<CalendarStrategyFactory>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<EventEnergyModeTempService>().AsSingle();
	}

	private void BindFactories()
	{
		base.Container.BindInterfacesAndSelfTo<EventFactory>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<BattlePassFactory>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<EventStructureInitializer>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<EventMapper>>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<BattlePassStructureInitializer>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<BattlePassMapper>>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<CurrencySpecialContentFactory>().When((InjectContext context) => context.ObjectType == typeof(LinkedContentFactory) && 1.Equals(context.ConcreteIdentifier));
		base.Container.BindInterfacesAndSelfTo<LinkedContentFactory>().WithConcreteId(1).WhenInjectedInto<EventFactory>();
	}

	private void BindLoaders()
	{
		base.Container.BindInterfacesAndSelfTo<EventBundleDataLoader>().AsSingle().WithArguments(BundleType.Events);
		base.Container.BindInterfacesAndSelfTo<EventShopBundleLoader>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<BattlePassBundleDataLoader>().AsSingle().WithArguments(BundleType.BattlePass);
		base.Container.BindInterfacesAndSelfTo<EventMergeIconsLoader>().AsSingle().WithArguments(BundleType.Events, "MergeItemIcons");
		base.Container.BindInterfacesAndSelfTo<BattlePassMergeIconsLoader>().AsSingle().WithArguments(BundleType.BattlePass, "MergeItemIcons");
	}

	private void BindManagers()
	{
		base.Container.Bind<LegacyMergeIconProvider>().AsSingle();
		base.Container.Bind<BattlePassMergeIconProvider>().AsSingle();
		base.Container.Bind<EventMergeIconProvider>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<EventMapperProvider>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<BattlePassMapperProvider>().AsSingle();
	}

	private void BindLightWeightFactories()
	{
		base.Container.BindInterfacesAndSelfTo<EventStrategyLightWeightFactory>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<BattlePassStrategyLightWeightFactory>().AsSingle();
	}

	private void BindEventBattlePassModules()
	{
		base.Container.BindInterfacesAndSelfTo<EventBattlePassChecker>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<EventBattlePassLoader>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<EventBattlePassDataBuilder>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<EventBattlePassDataCleaner>().AsSingle();
	}
}
