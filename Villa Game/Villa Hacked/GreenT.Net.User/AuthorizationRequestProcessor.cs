using System.Collections.Generic;

namespace GreenT.Net.User;

public sealed class AuthorizationRequestProcessor : BaseAuthorizationRequestProcessor
{
	public AuthorizationRequestProcessor(IPostRequest<Response<UserDataMapper>> postRequest, GreenT.User userData)
		: base(postRequest, userData)
	{
	}

	public void Request(string email, string password)
	{
		IDictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["email_by_pass"] = email;
		dictionary["email"] = email.FormatEmail();
		dictionary["password"] = password.FormatPassword();
		PostRequest(dictionary);
	}
}
