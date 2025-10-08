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
		return Observable.Catch<Response, Exception>(Observable.Select<string, Response>(HttpRequestExecutor.PostRequest(requestUrl, dictionary), (Func<string, Response>)JsonConvert.DeserializeObject<Response>), (Func<Exception, IObservable<Response>>)delegate(Exception ex)
		{
			throw ex.SendException("Exception on try to parse response by url :" + requestUrl);
		}).Debug("Email Check:", LogType.Net);
	}
}
