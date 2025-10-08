using GreenT.HornyScapes.BannerSpace;
using GreenT.HornyScapes.Data;
using GreenT.HornyScapes.Events.Content;
using Zenject;

namespace GreenT.HornyScapes.Bank;

public class BannerInstaller : Installer<BannerInstaller>
{
	public override void InstallBindings()
	{
		base.Container.BindIFactory<CreateDataManager>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<DataManagerCluster>().FromFactory<DataManagerCluster, ClusterFactory<CreateDataManager, DataManagerCluster>>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<BannerConfigInitializer>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<BannerMapper>>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<CreateDataFactory>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<BackgroundBundleLoader>().AsSingle();
		base.Container.Bind<BundleProvider>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<BannerCluster>().AsSingle();
		base.Container.Bind<SaveDataManager>().AsSingle();
		base.Container.Bind<BannerNotificationService>().AsSingle();
		base.Container.Bind<DropService>().AsSingle();
		base.Container.Bind<GreenT.HornyScapes.BannerSpace.Analytics>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<BannerInitializer>().AsSingle();
		base.Container.Bind<BannerFactory>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<BannerController>().AsSingle();
	}
}
