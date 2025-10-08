using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Data;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.Sellouts.Analytics;
using GreenT.HornyScapes.Sellouts.Factories;
using GreenT.HornyScapes.Sellouts.Loaders;
using GreenT.HornyScapes.Sellouts.Mappers;
using GreenT.HornyScapes.Sellouts.Providers;
using GreenT.HornyScapes.Sellouts.Services;
using GreenT.HornyScapes.Sellouts.StructureInitializers;
using GreenT.Settings.Data;
using Zenject;

namespace GreenT.HornyScapes.Sellouts;

public class SelloutInstaller : Installer<SelloutInstaller>
{
	public override void InstallBindings()
	{
		BindStructureInitializers();
		BindFactories();
		BindProviders();
		BindServices();
		BindCalendar();
		BindAnalytic();
	}

	private void BindStructureInitializers()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<SelloutRewardsStructureInitializer>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<SelloutRewardsMapper>>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<SelloutStructureInitializer>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<SelloutMapper>>()).AsSingle();
	}

	private void BindFactories()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<SelloutFactory>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<SelloutRewardsConditionFactory>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<SelloutRewardFactory>()).AsSingle();
	}

	private void BindProviders()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<SelloutManager>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<SelloutMapperProvider>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<SelloutRewardsMapperProvider>()).AsSingle();
	}

	private void BindServices()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<SelloutStateService>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<SelloutStateAnalytic>()).AsSingle();
		((ArgConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<SelloutDataLoader>()).AsSingle()).WithArguments<BundleType>(BundleType.Sellout);
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<SelloutEntryPoint>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<SelloutStateManager>()).AsSingle();
	}

	private void BindCalendar()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<SelloutCalendarLoader>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<SelloutStrategyLightWeightFactory>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<SelloutDataBuilder>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<SelloutCalendarDispenser>()).AsCached();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<SelloutDataCleaner>()).AsSingle();
	}

	private void BindAnalytic()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<SelloutAnalytic>()).AsSingle();
	}
}
