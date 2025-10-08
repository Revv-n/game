using System;

namespace GreenT.Net;

public class HTTPRequest<T> : HTTPGetRequest<T>, IGetRequest<T>, IRequest<T> where T : Response
{
	private readonly string requestUrl;

	public HTTPRequest(string requestUrl)
	{
		this.requestUrl = requestUrl;
	}

	public IObservable<T> Request(params string[] args)
	{
		return GetRequest(requestUrl, args);
	}
}
