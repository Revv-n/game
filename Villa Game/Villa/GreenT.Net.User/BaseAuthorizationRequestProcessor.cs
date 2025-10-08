namespace GreenT.Net.User;

public abstract class BaseAuthorizationRequestProcessor : UserPostRequestProcessor
{
	public BaseAuthorizationRequestProcessor(IPostRequest<Response<UserDataMapper>> postRequest, GreenT.User userData)
		: base(postRequest, userData)
	{
	}

	protected override void ProcessResponse(Response<UserDataMapper> response)
	{
		base.ProcessResponse(response);
		_ = response.Status;
	}
}
