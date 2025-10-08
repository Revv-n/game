using Zenject;

namespace GreenT.AssetBundles;

public class FakeAssetInstaller : MonoInstaller
{
	public FakeAssetsSO FakeCharacterBankImageses;

	public override void InstallBindings()
	{
		base.Container.Bind<FakeAssetProvider>().AsSingle();
		base.Container.Bind<FakeAssetService>().AsSingle();
		base.Container.Bind<FakeAssetsSO>().FromInstance(FakeCharacterBankImageses).AsSingle();
	}
}
