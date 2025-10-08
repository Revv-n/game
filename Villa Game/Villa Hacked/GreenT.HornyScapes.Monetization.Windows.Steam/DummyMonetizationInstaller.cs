using Zenject;

namespace GreenT.HornyScapes.Monetization.Windows.Steam;

public class DummyMonetizationInstaller : Installer<DummyMonetizationInstaller>
{
	private const string RegionName = "US";

	public override void InstallBindings()
	{
		((ArgConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesTo<DefaultRegionPriceResolver>()).AsSingle()).WithArguments<string>("US");
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<DummyMonetizationSystem>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<DefaultMonetizationRecorder>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<DummyGiftSystem>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<OfflinePaymentService>()).AsSingle();
	}
}
