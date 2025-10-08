using GreenT.Net.User;
using Zenject;

namespace GreenT.Data;

public class AuthorizationProcessorService : BaseAuthorizationProcessorService, IInitializable
{
	private readonly AuthorizationRequestProcessor authorizationProcessor;

	private readonly RestoreSessionProcessor restoreSessionProcessor;

	public AuthorizationProcessorService([InjectOptional] AuthorizationRequestProcessor authorizationProcessor, RestoreSessionProcessor restoreSessionProcessor, SaverState saverState)
		: base(saverState)
	{
		this.authorizationProcessor = authorizationProcessor;
		this.restoreSessionProcessor = restoreSessionProcessor;
	}

	public void Initialize()
	{
		authorizationProcessor?.AddListener(OnAuthorize, -2);
		restoreSessionProcessor.AddListener(OnAuthorize, -2);
		authorizationProcessor?.AddExceptionListener(base.OnError);
		restoreSessionProcessor.AddExceptionListener(base.OnError);
	}
}
