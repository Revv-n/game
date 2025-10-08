using GreenT.HornyScapes.Extensions;
using GreenT.HornyScapes.Steam;
using GreenT.Net;
using GreenT.Net.User;
using GreenT.Registration;
using GreenT.Settings.Data;
using Zenject;

namespace GreenT.HornyScapes.Net;

public class RequestInstaller : Installer<RequestInstaller>
{
	public const string SetData = "SetData";

	public const string GetData = "GetData";

	public const string AuthorizationData = "AuthorizationData";

	public static string DummyAuthKey = "DummyAuth";

	public static string PlayerIdKey => PlayerIdConstantProvider.GetPlayerId;

	public override void InstallBindings()
	{
		Installer<IndependentUrlInstaller>.Install(base.Container);
		BindRequestInstaller();
		BindRequests();
		BindProcessors();
		BindUserListeners();
	}

	private void BindRequestInstaller()
	{
	}

	private void BindRequests()
	{
		BindRemoteRequests();
	}

	private void BindRemoteRequests()
	{
		BindRequestWithId(PostRequestType.SetUserData, "SetData");
		BindRequestWithId(PostRequestType.GetUserData, "GetData");
		BindPlayerCheckRequest();
	}

	private void BindProcessors()
	{
		BindUserRequestProcessorAndSelf<SetDataProcessor>();
		BindUserRequestProcessorAndSelf<GetDataProcessor>();
		BindUserRequestProcessorAndSelf<RestoreSessionProcessor>().WithArguments(PlayerIdKey);
		base.Container.BindRequestProcessor<UserDataPostJsonRequest, SteamAuthorizationRequestProcessor>(PostRequestType.Authorization, "AuthorizationData");
		ConcreteIdArgConditionCopyNonLazyBinder BindUserRequestProcessorAndSelf<T>() where T : UserPostRequestProcessor
		{
			return base.Container.Bind(typeof(UserPostRequestProcessor), typeof(T)).To<T>().AsSingle();
		}
	}

	private void BindUserListeners()
	{
		Installer<UserRequestsListenerInstaller>.Install(base.Container);
	}

	private void BindRequestWithId(PostRequestType type, string key)
	{
		base.Container.ResolveUrlByType(type).When(OnEqualKey);
		base.Container.Bind<IPostRequest<Response<UserDataMapper>>>().WithId(key).To<UserDataPostRequest>()
			.AsCached();
		bool OnEqualKey(InjectContext _context)
		{
			if (_context.ParentContext.Identifier != null)
			{
				return _context.ParentContext.Identifier.Equals(key);
			}
			return false;
		}
	}

	private void BindPlayerCheckRequest()
	{
		base.Container.BindUrlWhenInjectedToType<PlayerCheckRequest>(PostRequestType.PlayerCheck);
		base.Container.BindInterfacesAndSelfTo<PlayerCheckRequest>().AsCached().WhenInjectedInto<RestoreSessionProcessor>();
	}
}
