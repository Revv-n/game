using Zenject;

namespace GreenT.HornyScapes;

public sealed class LastChanceCommonInstaller : Installer<LastChanceCommonInstaller>
{
	public override void InstallBindings()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<LastChanceManager>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<LastChanceFactory>()).AsSingle();
	}
}
