using System;
using System.Collections.Generic;
using GreenT.Net;
using Newtonsoft.Json;
using UniRx;

namespace GreenT.Registration;

public class EmailCheck : IEmailCheckRequest
{
	private readonly string requestUrl;

	public EmailCheck(string requestUrl)
	{
		this.requestUrl = requestUrl;
	}

	public IObservable<Response> Check(string mail)
	{
		IDictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["email"] = mail;
		return HttpRequestExecutor.PostRequest(requestUrl, dictionary).Select(JsonConvert.DeserializeObject<Response>).Catch(delegate(Exception ex)
		{
			throw ex.SendException("Exception on try to parse response by url :" + requestUrl);
		})
			.Debug("Email Check:", LogType.Net);
	}
}
