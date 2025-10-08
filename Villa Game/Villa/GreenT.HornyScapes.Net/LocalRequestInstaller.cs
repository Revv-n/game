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
		base.Container.BindRequestProcessor<FakeGetRequest, RegistrationRequestProcessor>(PostRequestType.Registration);
		base.Container.BindRequestProcessor<FakeGetRequest, AuthorizationRequestProcessor>(PostRequestType.Login);
	}

	private void BindFakePlayerCheckRequest()
	{
		base.Container.BindInterfacesAndSelfTo<FakeSuccessfulPlayerCheckRequest>().AsCached().WhenInjectedInto<RestoreSessionProcessor>();
	}

	private void BindFakeRequest()
	{
		base.Container.Bind<IPostRequest<Response<UserDataMapper>>>().WithId("SetData").To<SetFakeDataRequest>()
			.AsCached();
		base.Container.Bind<IPostRequest<Response<UserDataMapper>>>().WithId("GetData").To<FakeGetRequest>()
			.AsCached();
	}
}
