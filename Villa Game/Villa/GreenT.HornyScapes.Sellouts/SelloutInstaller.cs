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
		base.Container.BindInterfacesAndSelfTo<SelloutRewardsStructureInitializer>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<SelloutRewardsMapper>>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<SelloutStructureInitializer>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<SelloutMapper>>().AsSingle();
	}

	private void BindFactories()
	{
		base.Container.BindInterfacesAndSelfTo<SelloutFactory>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<SelloutRewardsConditionFactory>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<SelloutRewardFactory>().AsSingle();
	}

	private void BindProviders()
	{
		base.Container.BindInterfacesAndSelfTo<SelloutManager>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<SelloutMapperProvider>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<SelloutRewardsMapperProvider>().AsSingle();
	}

	private void BindServices()
	{
		base.Container.BindInterfacesAndSelfTo<SelloutStateService>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<SelloutStateAnalytic>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<SelloutDataLoader>().AsSingle().WithArguments(BundleType.Sellout);
		base.Container.BindInterfacesAndSelfTo<SelloutEntryPoint>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<SelloutStateManager>().AsSingle();
	}

	private void BindCalendar()
	{
		base.Container.BindInterfacesAndSelfTo<SelloutCalendarLoader>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<SelloutStrategyLightWeightFactory>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<SelloutDataBuilder>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<SelloutCalendarDispenser>().AsCached();
		base.Container.BindInterfacesAndSelfTo<SelloutDataCleaner>().AsSingle();
	}

	private void BindAnalytic()
	{
		base.Container.BindInterfacesAndSelfTo<SelloutAnalytic>().AsSingle();
	}
}
