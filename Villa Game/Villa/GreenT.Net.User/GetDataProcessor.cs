using System.Collections.Generic;
using Zenject;

namespace GreenT.Net.User;

public sealed class GetDataProcessor : UserPostRequestProcessor
{
	public GetDataProcessor([Inject(Id = "GetData")] IPostRequest<Response<UserDataMapper>> getUserData, GreenT.User userData)
		: base(getUserData, userData)
	{
	}

	public void Request(string playerID)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		if (!string.IsNullOrEmpty(playerID))
		{
			dictionary["player_id"] = playerID;
		}
		PostRequest(dictionary);
	}
}
