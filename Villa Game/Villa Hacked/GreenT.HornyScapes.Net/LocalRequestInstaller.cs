using GreenT.HornyScapes.Extensions;
using GreenT.Net;
using GreenT.Net.User;
using GreenT.Registration.Test;
using GreenT.Settings.Data;
using Zenject;

namespace GreenT.HornyScapes.Net;

public class LocalRequestInstaller : Installer<LocalRequestInstaller>
{
	public override void InstallBindings()
	{
		BindFakeRequest();
		BindFakePlayerCheckRequest();
		((InstallerBase)this).Container.BindRequestProcessor<FakeGetRequest, RegistrationRequestProcessor>(PostRequestType.Registration);
		((InstallerBase)this).Container.BindRequestProcessor<FakeGetRequest, AuthorizationRequestProcessor>(PostRequestType.Login);
	}

	private void BindFakePlayerCheckRequest()
	{
		((ConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<FakeSuccessfulPlayerCheckRequest>()).AsCached()).WhenInjectedInto<RestoreSessionProcessor>();
	}

	private void BindFakeRequest()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.Bind<IPostRequest<Response<UserDataMapper>>>().WithId((object)"SetData").To<SetFakeDataRequest>()).AsCached();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.Bind<IPostRequest<Response<UserDataMapper>>>().WithId((object)"GetData").To<FakeGetRequest>()).AsCached();
	}
}
