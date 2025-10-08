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
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<DateIconDataLoader>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<DateStoryLoader>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<DateBackgroundDataLoader>()).AsSingle();
	}

	private void BindServices()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.Bind<DateUnlockService>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<DateLoadService>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.Bind<DateIconDataLoadService>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.Bind<DateSaveRestoreService>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.Bind<DateUnloadService>()).AsSingle();
	}

	private void BindFactories()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.Bind<DateFactory>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.Bind<DatePhraseFactory>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<DateLinkedContentFactory>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<ComingSoonDateLinkedContentFactory>()).AsSingle();
	}

	private void BindProviders()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.Bind<DateProvider>()).AsSingle();
	}

	private void BindStructureInitializers()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<DateStructureInitializer>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<DatePhraseMapper>>()).AsSingle();
	}
}
