using System;
using System.Collections.Generic;
using GreenT.Net;
using Newtonsoft.Json;
using UniRx;

namespace GreenT.Registration.Test;

public class FakeGetRequest : IPostRequest<Response<UserDataMapper>>, IPostRequest<Response<UserDataMapper>, IDictionary<string, string>>
{
	public IObservable<Response<UserDataMapper>> Post(IDictionary<string, string> fields)
	{
		fields.TryGetValue("email", out var value);
		if (!fields.TryGetValue("player_id", out var value2))
		{
			value2 = "100000000000000000000001";
		}
		return Observable.Return<Response<UserDataMapper>>(JsonConvert.DeserializeObject<Response<UserDataMapper>>("{\"status\":\"0\", \"error\":\"\", \"data\":{ \"user_name\":\"Fake Login\",\"email\":\"" + value + "\",\"data\":\"\",\"data_compressed\":0,\"player_id\":\"" + value2 + "\",\"updated_at\":" + DateTimeOffset.Now.ToUnixTimeSeconds() + " }")).Debug("FAKE GET RESPONSE", LogType.Net);
	}
}
