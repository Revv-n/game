using System;
using System.Collections.Generic;
using GreenT.Net;

namespace GreenT.HornyScapes.Monetization.Net;

public class PaymentGetReuqest<T>
{
	private static readonly Dictionary<string, string> headers;

	private string url;

	static PaymentGetReuqest()
	{
		headers = new Dictionary<string, string>
		{
			{ "Access-Control-Allow-Credentials", "true" },
			{ "Access-Control-Allow-Headers", "Access-Control-Allow-Headers, Origin, Accept, X-Access-Token, X-Application-Name, X-Request-Sent-Time, X-Requested-With, Content-Type, Access-Control-Request-Method, Access-Control-Request-Headers, Access-Control-Allow-Credentials" },
			{ "Access-Control-Allow-Methods", "POST, GET, OPTIONS, PUT, DELETE" },
			{ "Access-Control-Allow-Origin", "*" }
		};
	}

	public PaymentGetReuqest(string url)
	{
		this.url = url;
	}

	public IObservable<T> Request(params string[] parameters)
	{
		return HttpRequestExecutor.GetRequest<T>(string.Format(url, parameters), cached: false, headers);
	}
}
