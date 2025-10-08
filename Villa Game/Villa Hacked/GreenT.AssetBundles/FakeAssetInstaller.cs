using Zenject;

namespace GreenT.AssetBundles;

public class FakeAssetInstaller : MonoInstaller
{
	public FakeAssetsSO FakeCharacterBankImageses;

	public override void InstallBindings()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<FakeAssetProvider>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<FakeAssetService>()).AsSingle();
		((FromBinderGeneric<FakeAssetsSO>)(object)((MonoInstallerBase)this).Container.Bind<FakeAssetsSO>()).FromInstance(FakeCharacterBankImageses).AsSingle();
	}
}
