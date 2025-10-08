using System;

namespace GreenT.Net;

public class HTTPConcretteGetRequestNoRespone<T> : HTTPGetRequestNoResponse<T>
{
	private string requestUrl;

	public HTTPConcretteGetRequestNoRespone(string url)
	{
		requestUrl = url;
	}

	public IObservable<T> GetRequest(params object[] args)
	{
		return base.GetRequest(requestUrl, args);
	}
}
