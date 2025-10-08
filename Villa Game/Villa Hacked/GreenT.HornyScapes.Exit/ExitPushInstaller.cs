using GreenT.UI;
using Zenject;

namespace GreenT.HornyScapes.Exit;

public class ExitPushInstaller : MonoInstaller
{
	public WindowGroupID ExitPreset;

	public override void InstallBindings()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<ExitPushController>()).AsSingle();
		((ArgConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<ExitPopupOpener>()).AsSingle()).WithArguments<WindowGroupID>(ExitPreset);
	}
}
