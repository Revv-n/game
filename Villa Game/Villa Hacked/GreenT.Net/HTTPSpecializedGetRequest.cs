using System;
using System.Collections.Generic;

namespace GreenT.Net;

public class HTTPSpecializedGetRequest<T>
{
	public IObservable<T> GetRequest(string requestUrl, IEnumerable<KeyValuePair<string, string>> headers = null, params string[] args)
	{
		return HttpRequestExecutor.GetRequest<T>(string.Format(requestUrl, args), cached: false, headers);
	}
}
