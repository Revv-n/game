namespace GreenT.Net.User;

public class UserDataPostJsonRequest : PostJsonRequest<Response<UserDataMapper>>
{
	public UserDataPostJsonRequest(string url)
		: base(url)
	{
	}
}
