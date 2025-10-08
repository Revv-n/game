using System.Collections.Generic;
using Zenject;

namespace GreenT.Net.User;

public class TokenAuthorizationRequestProcessor : BaseAuthorizationRequestProcessor
{
	public TokenAuthorizationRequestProcessor(GreenT.User userData, [Inject(Id = "AuthorizationData")] IPostRequest<Response<UserDataMapper>> postRequest)
		: base(postRequest, userData)
	{
	}

	public void Auth(string token)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["token"] = token;
		PostRequest(dictionary);
	}
}
