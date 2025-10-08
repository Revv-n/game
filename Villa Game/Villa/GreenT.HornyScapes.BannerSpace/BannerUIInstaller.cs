using ModestTree;
using Zenject;

namespace GreenT.HornyScapes.BannerSpace;

public class BannerUIInstaller : MonoInstaller<BannerUIInstaller>
{
	public BannerBackgroundContainer BannerBackgroundContainer;

	public override void InstallBindings()
	{
		Assert.IsNotNull(BannerBackgroundContainer);
		base.Container.Bind<BannerBackgroundContainer>().FromInstance(BannerBackgroundContainer).AsSingle();
	}
}
