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
		((ArgConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<LocalStorageSetter>()).AsSingle()).WithArguments<string>(PlayerIdConstantProvider.GetPlayerId);
	}

	private void BindEmailCheckRequest()
	{
		((InstallerBase)this).Container.BindUrlWhenInjectedToType<EmailCheck>(PostRequestType.EmailCheck);
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((ConcreteBinderGeneric<IEmailCheckRequest>)(object)((InstallerBase)this).Container.Bind<IEmailCheckRequest>()).To<EmailCheck>()).AsSingle();
	}
}
