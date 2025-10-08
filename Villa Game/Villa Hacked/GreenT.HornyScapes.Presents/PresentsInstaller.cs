using GreenT.HornyScapes.Data;
using GreenT.HornyScapes.Presents.Factories;
using GreenT.HornyScapes.Presents.Loaders;
using GreenT.HornyScapes.Presents.Managers;
using GreenT.HornyScapes.Presents.Services;
using Zenject;

namespace GreenT.HornyScapes.Presents;

public class PresentsInstaller : Installer<PresentsInstaller>
{
	public override void InstallBindings()
	{
		BindInitializers();
		BindFactories();
		BindServices();
		BindLoaders();
		BindManagers();
	}

	private void BindServices()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.Bind<PresentsWindowOpener>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.Bind<PresentsViewTapTracker>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<PresentsService>()).AsSingle();
	}

	private void BindFactories()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesTo<PresentsFactory>()).AsSingle();
	}

	private void BindInitializers()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<PresentsStructureInitializer>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<PresentsMapper>>()).AsSingle();
	}

	private void BindLoaders()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<PresentsBundleLoader>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.Bind<PresentsBundleLoadService>()).AsSingle();
	}

	private void BindManagers()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<PresentsManager>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<PresentBundleManager>()).AsSingle();
	}
}
