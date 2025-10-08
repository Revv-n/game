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
		Installer<SteamAchievementInstaller>.Install(((InstallerBase)this).Container);
	}

	private void BindServices()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<SteamBridge>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<SteamSDKService>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<SteamWarningMessageHandler>()).AsSingle();
	}

	private void BindAuthorizations()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<SteamPaymentAuthorization>()).AsSingle();
		((ArgConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<SteamAuthTicketForWebApi>()).AsSingle()).WithArguments<string>(PlayerId);
	}

	private void BindMix()
	{
		((NonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<SteamCallbackRunner>()).AsSingle()).NonLazy();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<SteamNativeMethods>()).AsSingle();
	}
}
