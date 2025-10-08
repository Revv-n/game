using Zenject;

namespace GreenT.HornyScapes.Settings;

public class SettingsPushInstaller : Installer<SettingsPushInstaller>
{
	public override void InstallBindings()
	{
		base.Container.BindInterfacesAndSelfTo<SettingsPushController>().AsSingle();
	}
}
