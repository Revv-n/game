using Zenject;

namespace GreenT.Bonus;

public class BonusInstaller : Installer<BonusInstaller>
{
	public override void InstallBindings()
	{
		((NonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<BonusController>()).AsSingle()).NonLazy();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<BonusFactory>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.Bind<BonusManager>()).AsSingle();
	}
}
