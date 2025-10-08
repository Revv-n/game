using System;
using System.Collections.Generic;
using GreenT.Net;
using Newtonsoft.Json;
using UniRx;

namespace GreenT.Registration;

public class PlayerCheckRequest : IPostRequest<Response<UserDataMapper>, string>
{
	private readonly string requestUrl;

	public PlayerCheckRequest(string requestUrl)
	{
		this.requestUrl = requestUrl;
	}

	public IObservable<Response<UserDataMapper>> Post(string playerID)
	{
		IDictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["player_id"] = playerID;
		return Observable.Catch<Response<UserDataMapper>, Exception>(Observable.Select<string, Response<UserDataMapper>>(HttpRequestExecutor.PostRequest(requestUrl, dictionary), (Func<string, Response<UserDataMapper>>)JsonConvert.DeserializeObject<Response<UserDataMapper>>), (Func<Exception, IObservable<Response<UserDataMapper>>>)delegate(Exception ex)
		{
			throw ex.SendException(playerID);
		});
	}
}
