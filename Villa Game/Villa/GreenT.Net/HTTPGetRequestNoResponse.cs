using System;

namespace GreenT.Net;

public class HTTPGetRequestNoResponse<T> : IGetRequestNoResponse<T>
{
	public IObservable<T> GetRequest(string requestUrl, params object[] args)
	{
		return HttpRequestExecutor.GetRequest<T>(string.Format(requestUrl, args));
	}
}
