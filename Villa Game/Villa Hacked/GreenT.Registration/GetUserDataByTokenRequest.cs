using System;
using System.Collections.Generic;
using GreenT.Net;
using Newtonsoft.Json;
using UniRx;

namespace GreenT.Registration;

public class GetUserDataByTokenRequest : IPostRequest<Response<UserDataMapper>, string>
{
	private readonly string TOKEN_FIELD_NAME = "token";

	private readonly string requestUrl;

	public GetUserDataByTokenRequest(string requestUrl)
	{
		this.requestUrl = requestUrl;
	}

	public IObservable<Response<UserDataMapper>> Post(string token)
	{
		return Observable.Select<string, Response<UserDataMapper>>(HttpRequestExecutor.PostRequest(string.Format(requestUrl, token), new Dictionary<string, string> { [TOKEN_FIELD_NAME] = token }), (Func<string, Response<UserDataMapper>>)JsonConvert.DeserializeObject<Response<UserDataMapper>>);
	}
}
