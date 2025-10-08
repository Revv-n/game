using GreenT.HornyScapes.Data;
using GreenT.HornyScapes.Relationships.Factories;
using GreenT.HornyScapes.Relationships.Mappers;
using GreenT.HornyScapes.Relationships.Providers;
using GreenT.HornyScapes.Relationships.Services;
using GreenT.HornyScapes.Relationships.StructureInitializers;
using Zenject;

namespace GreenT.HornyScapes.Relationships;

public class RelationshipInstaller : Installer<RelationshipInstaller>
{
	public override void InstallBindings()
	{
		BindStructureInitializers();
		BindFactories();
		BindProviders();
		BindServices();
	}

	private void BindStructureInitializers()
	{
		base.Container.BindInterfacesAndSelfTo<RelationshipRewardsStructureInitializer>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<RelationshipRewardMapper>>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<RelationshipStructureInitializer>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<RelationshipMapper>>().AsSingle();
	}

	private void BindFactories()
	{
		base.Container.Bind<RelationshipFactory>().AsSingle();
		base.Container.Bind<RelationshipRewardConditionFactory>().AsSingle();
		base.Container.Bind<RelationshipRewardFactory>().AsSingle();
		base.Container.Bind<LevelUpCommandFactory>().AsSingle();
	}

	private void BindProviders()
	{
		base.Container.Bind<RelationshipProvider>().AsSingle();
		base.Container.Bind<RelationshipMapperProvider>().AsSingle();
		base.Container.Bind<RelationshipRewardMapperProvider>().AsSingle();
		base.Container.Bind<LevelUpCommandStorage>().AsSingle();
	}

	private void BindServices()
	{
		base.Container.BindInterfacesAndSelfTo<RelationshipLevelTracker>().AsSingle();
		base.Container.Bind<RelationshipLevelService>().AsSingle();
		base.Container.Bind<BlockLevelTracker>().AsSingle();
		base.Container.Bind<RelationshipStatusTracker>().AsSingle();
	}
}
