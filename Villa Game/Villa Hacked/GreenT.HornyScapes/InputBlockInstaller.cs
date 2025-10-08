using Zenject;

namespace GreenT.HornyScapes;

public class InputBlockInstaller : Installer<InputBlockInstaller>
{
	public override void InstallBindings()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesTo<InputBlockController>()).AsSingle();
	}
}
