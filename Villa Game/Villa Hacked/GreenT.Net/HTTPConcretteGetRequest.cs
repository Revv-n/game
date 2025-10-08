using System;

namespace GreenT.Net;

public class HTTPConcretteGetRequest<T> : HTTPGetRequest<T> where T : Response
{
	private string requestUrl;

	public HTTPConcretteGetRequest(string url)
	{
		requestUrl = url;
	}

	public IObservable<T> GetRequest(params object[] args)
	{
		return base.GetRequest(requestUrl, args);
	}
}
