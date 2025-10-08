using System.Collections.Generic;

namespace GreenT.Net.User;

public sealed class RegistrationRequestProcessor : UserPostRequestProcessor
{
	public RegistrationRequestProcessor(IPostRequest<Response<UserDataMapper>> postRequest, GreenT.User userData)
		: base(postRequest, userData)
	{
	}

	public void Request(string email, string password)
	{
		IDictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["email"] = email.FormatEmail();
		dictionary["password"] = password.FormatPassword();
		if (!string.IsNullOrEmpty(user.PlayerID))
		{
			dictionary["player_id"] = user.PlayerID;
		}
		PostRequest(dictionary);
	}

	protected override void ProcessResponse(Response<UserDataMapper> response)
	{
		base.ProcessResponse(response);
		_ = response.Status;
	}
}
