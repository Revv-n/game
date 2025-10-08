using System;
using System.Collections.Generic;
using Zenject;

namespace GreenT.Net.User;

public sealed class SetDataProcessor : UserPostRequestProcessor
{
	public SetDataProcessor([Inject(Id = "SetData")] IPostRequest<Response<UserDataMapper>> setDataRequest, GreenT.User userData)
		: base(setDataRequest, userData)
	{
	}

	public void Request(string playerID, string userName, byte[] data = null)
	{
		IDictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["player_id"] = playerID;
		dictionary["user_name"] = userName;
		if (data != null)
		{
			dictionary["data"] = Convert.ToBase64String(data);
		}
		PostRequest(dictionary);
	}
}
