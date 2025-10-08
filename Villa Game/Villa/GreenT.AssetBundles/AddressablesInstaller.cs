using Zenject;

namespace GreenT.AssetBundles;

public class AddressablesInstaller : Installer<AddressablesInstaller>
{
	public override void InstallBindings()
	{
		base.Container.Bind<AddressableAssetLoader>().AsSingle();
		base.Container.Bind<AddressableBundleLoader>().AsSingle();
		base.Container.Bind<AddressableResourceLocationProvider>().AsSingle();
		base.Container.Bind<IAddressablesBundlesLoader>().To<AddressablesService>().AsSingle();
		base.Container.Bind<AddressableBundleCache>().AsSingle();
		base.Container.Bind<AddressableAssetCache>().AsSingle();
	}
}
