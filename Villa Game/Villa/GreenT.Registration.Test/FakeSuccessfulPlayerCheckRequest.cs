using System;
using GreenT.Net;
using Newtonsoft.Json;
using UniRx;

namespace GreenT.Registration.Test;

public class FakeSuccessfulPlayerCheckRequest : IPostRequest<Response<UserDataMapper>, string>
{
	public IObservable<Response<UserDataMapper>> Post(string playerID)
	{
		return Observable.Return(JsonConvert.DeserializeObject<Response<UserDataMapper>>("{\"status\":200,\"error\":\"\",\"data\":\"\"}"));
	}
}
