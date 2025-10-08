namespace GreenT.Net.User;

public class UserDataPostRequest : PostRequest<Response<UserDataMapper>>
{
	public UserDataPostRequest(string url)
		: base(url)
	{
	}
}
