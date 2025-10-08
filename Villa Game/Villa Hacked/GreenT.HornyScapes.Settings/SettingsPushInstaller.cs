using Zenject;

namespace GreenT.HornyScapes.Settings;

public class SettingsPushInstaller : Installer<SettingsPushInstaller>
{
	public override void InstallBindings()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<SettingsPushController>()).AsSingle();
	}
}
