using Zenject;

namespace GreenT.Steam;

public class DummySteamBridgeInstaller : Installer<DummySteamBridgeInstaller>
{
	public override void InstallBindings()
	{
		base.Container.BindInterfacesAndSelfTo<DummySteamBridge>().AsSingle();
		Installer<SteamAchievementInstaller>.Install(base.Container);
	}
}
