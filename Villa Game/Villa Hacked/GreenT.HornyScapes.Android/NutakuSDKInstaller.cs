using GreenT.HornyScapes.Net;
using GreenT.HornyScapes.Nutaku.Android;
using Zenject;

namespace GreenT.HornyScapes.Android;

public class NutakuSDKInstaller : Installer<NutakuSDKInstaller>
{
	public override void InstallBindings()
	{
		((ArgConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.Bind<NutakuAuthorization>()).AsSingle()).WithArguments<string>(PlayerIdConstantProvider.GetPlayerId);
	}
}
