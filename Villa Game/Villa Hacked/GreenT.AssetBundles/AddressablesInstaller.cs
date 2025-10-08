using Zenject;

namespace GreenT.AssetBundles;

public class AddressablesInstaller : Installer<AddressablesInstaller>
{
	public override void InstallBindings()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.Bind<AddressableAssetLoader>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.Bind<AddressableBundleLoader>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.Bind<AddressableResourceLocationProvider>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((ConcreteBinderGeneric<IAddressablesBundlesLoader>)(object)((InstallerBase)this).Container.Bind<IAddressablesBundlesLoader>()).To<AddressablesService>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.Bind<AddressableBundleCache>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.Bind<AddressableAssetCache>()).AsSingle();
	}
}
