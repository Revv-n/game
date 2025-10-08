using System;
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
		Installer<IndependentUrlInstaller>.Install(((InstallerBase)this).Container);
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
		((ArgConditionCopyNonLazyBinder)BindUserRequestProcessorAndSelf<RestoreSessionProcessor>()).WithArguments<string>(PlayerIdKey);
		((InstallerBase)this).Container.BindRequestProcessor<UserDataPostJsonRequest, SteamAuthorizationRequestProcessor>(PostRequestType.Authorization, "AuthorizationData");
		ConcreteIdArgConditionCopyNonLazyBinder BindUserRequestProcessorAndSelf<T>() where T : UserPostRequestProcessor
		{
			return ((ScopeConcreteIdArgConditionCopyNonLazyBinder)((ConcreteBinderNonGeneric)((InstallerBase)this).Container.Bind(new Type[2]
			{
				typeof(UserPostRequestProcessor),
				typeof(T)
			})).To<T>()).AsSingle();
		}
	}

	private void BindUserListeners()
	{
		Installer<UserRequestsListenerInstaller>.Install(((InstallerBase)this).Container);
	}

	private void BindRequestWithId(PostRequestType type, string key)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		((ConditionCopyNonLazyBinder)((InstallerBase)this).Container.ResolveUrlByType(type)).When(new BindingCondition(OnEqualKey));
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.Bind<IPostRequest<Response<UserDataMapper>>>().WithId((object)key).To<UserDataPostRequest>()).AsCached();
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
		((InstallerBase)this).Container.BindUrlWhenInjectedToType<PlayerCheckRequest>(PostRequestType.PlayerCheck);
		((ConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<PlayerCheckRequest>()).AsCached()).WhenInjectedInto<RestoreSessionProcessor>();
	}
}
