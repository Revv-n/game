namespace GreenT.Net.User;

public class UserPostRequestProcessor : PostRequestProcessor<Response<UserDataMapper>>
{
	protected GreenT.User user;

	public UserPostRequestProcessor(IPostRequest<Response<UserDataMapper>> postRequest, GreenT.User userData)
		: base(postRequest)
	{
		user = userData;
	}
}
