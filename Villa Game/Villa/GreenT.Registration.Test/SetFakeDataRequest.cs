using System;
using System.Collections.Generic;
using GreenT.Net;
using Newtonsoft.Json;
using UniRx;
using Zenject;

namespace GreenT.Registration.Test;

public class SetFakeDataRequest : IPostRequest<Response<UserDataMapper>>, IPostRequest<Response<UserDataMapper>, IDictionary<string, string>>
{
	[Inject]
	private User user;

	public IObservable<Response<UserDataMapper>> Post(IDictionary<string, string> fields)
	{
		if (!fields.TryGetValue("player_id", out var value))
		{
			value = "100000000000000000000001";
		}
		if (!fields.TryGetValue("user_name", out var value2))
		{
			value2 = string.Empty;
		}
		if (!fields.TryGetValue("email", out var value3))
		{
			value3 = user.Email?.ToString() ?? string.Empty;
		}
		return Observable.Return(JsonConvert.DeserializeObject<Response<UserDataMapper>>("{\"status\":\"0\", \"error\":\"\", \"data\":{ \"user_name\":\"" + value2 + "\",\"email\":\"" + value3 + "\",\"data\":\"\",\"data_compressed\":0,\"player_id\":\"" + value + "\",\"updated_at\":" + DateTimeOffset.Now.ToUnixTimeSeconds() + " }")).Debug("FAKE SET REQUEST", LogType.Net);
	}
}
