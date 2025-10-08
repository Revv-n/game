using Zenject;

namespace GreenT.HornyScapes.Monetization.Windows.Steam;

public class DummyMonetizationInstaller : Installer<DummyMonetizationInstaller>
{
	private const string RegionName = "US";

	public override void InstallBindings()
	{
		base.Container.BindInterfacesTo<DefaultRegionPriceResolver>().AsSingle().WithArguments("US");
		base.Container.BindInterfacesAndSelfTo<DummyMonetizationSystem>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<DefaultMonetizationRecorder>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<DummyGiftSystem>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<OfflinePaymentService>().AsSingle();
	}
}
