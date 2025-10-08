using GreenT.HornyScapes.Extensions;
using GreenT.Registration;
using GreenT.Settings.Data;
using Zenject;

namespace GreenT.HornyScapes.Net;

public class LocalStorageMailInstaller : Installer<LocalStorageMailInstaller>
{
	public override void InstallBindings()
	{
		BindEmailCheckRequest();
		BindLocalStorageSetter();
	}

	private void BindLocalStorageSetter()
	{
		base.Container.BindInterfacesAndSelfTo<LocalStorageSetter>().AsSingle().WithArguments(PlayerIdConstantProvider.GetPlayerId);
	}

	private void BindEmailCheckRequest()
	{
		base.Container.BindUrlWhenInjectedToType<EmailCheck>(PostRequestType.EmailCheck);
		base.Container.Bind<IEmailCheckRequest>().To<EmailCheck>().AsSingle();
	}
}
