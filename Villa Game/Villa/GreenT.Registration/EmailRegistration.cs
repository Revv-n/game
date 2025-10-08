using System;
using System.Collections.Generic;
using GreenT.Net;
using Newtonsoft.Json;
using UniRx;
using Zenject;

namespace GreenT.Registration;

public class EmailRegistration : IPostRequest<Response<UserDataMapper>>, IPostRequest<Response<UserDataMapper>, IDictionary<string, string>>
{
	private readonly string requestUrl;

	private readonly IEmailCheckRequest emailCheckReuqest;

	private readonly IPostRequest<Response<UserDataMapper>> getUserData;

	public EmailRegistration(IEmailCheckRequest emailCheckReuqest, [Inject(Id = "GetData")] IPostRequest<Response<UserDataMapper>> getUserData, string registrationUrl)
	{
		this.emailCheckReuqest = emailCheckReuqest;
		this.getUserData = getUserData;
		requestUrl = registrationUrl;
	}

	public IObservable<Response<UserDataMapper>> Post(IDictionary<string, string> fields)
	{
		if (!fields.ContainsKey("email"))
		{
			throw new ArgumentNullException("Field \"mail\" must be set");
		}
		if (!fields.ContainsKey("password"))
		{
			throw new ArgumentNullException("Field \"password\" must be set");
		}
		IObservable<Response> source = from _response in emailCheckReuqest.Check(fields["email"])
			where _response.Status.Equals(0)
			select _response;
		if (!fields.ContainsKey("player_id") || string.IsNullOrEmpty(fields["player_id"]))
		{
			Dictionary<string, string> fields2 = new Dictionary<string, string>();
			source = source.ContinueWith(getUserData.Post(fields2)).Do(delegate(Response<UserDataMapper> _response)
			{
				fields["player_id"] = _response.Data.PlayerID;
			});
		}
		return source.ContinueWith(HttpRequestExecutor.PostRequest(requestUrl, fields)).Select(JsonConvert.DeserializeObject<Response<UserDataMapper>>).Catch(delegate(Exception ex)
		{
			ex.LogException();
			throw ex;
		});
	}
}
