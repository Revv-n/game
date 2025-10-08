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
		base.Container.Bind<PresentsWindowOpener>().AsSingle();
		base.Container.Bind<PresentsViewTapTracker>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<PresentsService>().AsSingle();
	}

	private void BindFactories()
	{
		base.Container.BindInterfacesTo<PresentsFactory>().AsSingle();
	}

	private void BindInitializers()
	{
		base.Container.BindInterfacesAndSelfTo<PresentsStructureInitializer>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<PresentsMapper>>().AsSingle();
	}

	private void BindLoaders()
	{
		base.Container.BindInterfacesAndSelfTo<PresentsBundleLoader>().AsSingle();
		base.Container.Bind<PresentsBundleLoadService>().AsSingle();
	}

	private void BindManagers()
	{
		base.Container.BindInterfacesAndSelfTo<PresentsManager>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<PresentBundleManager>().AsSingle();
	}
}
