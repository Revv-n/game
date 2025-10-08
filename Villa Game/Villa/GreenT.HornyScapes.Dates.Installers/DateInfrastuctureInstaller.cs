using GreenT.HornyScapes.Data;
using GreenT.HornyScapes.Dates.Factories;
using GreenT.HornyScapes.Dates.Loaders;
using GreenT.HornyScapes.Dates.Mappers;
using GreenT.HornyScapes.Dates.Providers;
using GreenT.HornyScapes.Dates.Services;
using GreenT.HornyScapes.Dates.StructureInitializers;
using Zenject;

namespace GreenT.HornyScapes.Dates.Installers;

public class DateInfrastuctureInstaller : Installer<DateInfrastuctureInstaller>
{
	public override void InstallBindings()
	{
		BindLoaders();
		BindServices();
		BindFactories();
		BindProviders();
		BindStructureInitializers();
	}

	private void BindLoaders()
	{
		base.Container.BindInterfacesAndSelfTo<DateIconDataLoader>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<DateStoryLoader>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<DateBackgroundDataLoader>().AsSingle();
	}

	private void BindServices()
	{
		base.Container.Bind<DateUnlockService>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<DateLoadService>().AsSingle();
		base.Container.Bind<DateIconDataLoadService>().AsSingle();
		base.Container.Bind<DateSaveRestoreService>().AsSingle();
		base.Container.Bind<DateUnloadService>().AsSingle();
	}

	private void BindFactories()
	{
		base.Container.Bind<DateFactory>().AsSingle();
		base.Container.Bind<DatePhraseFactory>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<DateLinkedContentFactory>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<ComingSoonDateLinkedContentFactory>().AsSingle();
	}

	private void BindProviders()
	{
		base.Container.Bind<DateProvider>().AsSingle();
	}

	private void BindStructureInitializers()
	{
		base.Container.BindInterfacesAndSelfTo<DateStructureInitializer>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<DatePhraseMapper>>().AsSingle();
	}
}
