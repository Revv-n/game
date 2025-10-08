using GreenT.HornyScapes.Net;
using Zenject;

namespace GreenT.Steam;

public class SteamBridgeInstaller : Installer<SteamBridgeInstaller>
{
	private string PlayerId => PlayerIdConstantProvider.GetPlayerId;

	public override void InstallBindings()
	{
		BindServices();
		BindAuthorizations();
		BindMix();
		Installer<SteamAchievementInstaller>.Install(base.Container);
	}

	private void BindServices()
	{
		base.Container.BindInterfacesAndSelfTo<SteamBridge>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<SteamSDKService>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<SteamWarningMessageHandler>().AsSingle();
	}

	private void BindAuthorizations()
	{
		base.Container.BindInterfacesAndSelfTo<SteamPaymentAuthorization>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<SteamAuthTicketForWebApi>().AsSingle().WithArguments(PlayerId);
	}

	private void BindMix()
	{
		base.Container.BindInterfacesAndSelfTo<SteamCallbackRunner>().AsSingle().NonLazy();
		base.Container.BindInterfacesAndSelfTo<SteamNativeMethods>().AsSingle();
	}
}
