using GreenT.HornyScapes.Net;
using GreenT.HornyScapes.Nutaku.Android;
using Zenject;

namespace GreenT.HornyScapes.Android;

public class NutakuSDKInstaller : Installer<NutakuSDKInstaller>
{
	public override void InstallBindings()
	{
		base.Container.Bind<NutakuAuthorization>().AsSingle().WithArguments(PlayerIdConstantProvider.GetPlayerId);
	}
}
