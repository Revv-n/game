using ModestTree;
using Zenject;

namespace GreenT.HornyScapes.BannerSpace;

public class BannerUIInstaller : MonoInstaller<BannerUIInstaller>
{
	public BannerBackgroundContainer BannerBackgroundContainer;

	public override void InstallBindings()
	{
		Assert.IsNotNull((object)BannerBackgroundContainer);
		((FromBinderGeneric<BannerBackgroundContainer>)(object)((MonoInstallerBase)this).Container.Bind<BannerBackgroundContainer>()).FromInstance(BannerBackgroundContainer).AsSingle();
	}
}
