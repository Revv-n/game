using System;

namespace GreenT.Net;

public class HTTPGetRequest<T> : IGetRequest<T> where T : Response
{
	public IObservable<T> GetRequest(string requestUrl, params object[] args)
	{
		return HttpRequestExecutor.GetRequest<T>(string.Format(requestUrl, args));
	}
}
public class HTTPGetRequest : IGetRequest
{
	public IObservable<T> GetRequest<T>(string requestUrl, params object[] args) where T : Response
	{
		return HttpRequestExecutor.GetRequest<T>(string.Format(requestUrl, args));
	}
}
