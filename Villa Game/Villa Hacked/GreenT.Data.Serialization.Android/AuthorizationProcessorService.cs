using GreenT.Net.User;
using Zenject;

namespace GreenT.Data.Serialization.Android;

public class AuthorizationProcessorService : BaseAuthorizationProcessorService, IInitializable
{
	private RestoreSessionProcessor restoreSessionProcessor;

	public AuthorizationProcessorService(RestoreSessionProcessor restoreSessionProcessor, SaverState saverState)
		: base(saverState)
	{
		this.restoreSessionProcessor = restoreSessionProcessor;
	}

	public void Initialize()
	{
		restoreSessionProcessor.AddListener(OnAuthorize, -2);
		restoreSessionProcessor.AddExceptionListener(base.OnError);
	}
}
