using GreenT.HornyScapes.Events;
using Zenject;

namespace GreenT.HornyScapes;

public sealed class LastChanceInstaller : MonoInstaller<LastChanceInstaller>
{
	public override void InstallBindings()
	{
		BindStrategies();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<LastChanceEventBundleProvider>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<LastChanceController>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<LastChanceInitializer>()).AsSingle();
	}

	private void BindStrategies()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<LastChanceRatingsStrategy>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<LastChanceEventBattlePassStrategy>()).AsSingle();
	}
}
