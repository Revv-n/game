using Zenject;

namespace GreenT.Steam;

public class DummySteamBridgeInstaller : Installer<DummySteamBridgeInstaller>
{
	public override void InstallBindings()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<DummySteamBridge>()).AsSingle();
		Installer<SteamAchievementInstaller>.Install(((InstallerBase)this).Container);
	}
}
