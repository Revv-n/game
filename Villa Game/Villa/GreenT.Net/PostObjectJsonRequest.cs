using System.Collections.Generic;

namespace GreenT.Net;

public class PostObjectJsonRequest<TResponse> : PostJsonRequest<TResponse, IDictionary<string, object>>, IObjectPostRequest<TResponse>, IPostRequest<TResponse, IDictionary<string, object>>
{
	public PostObjectJsonRequest(string requestUrl)
		: base(requestUrl)
	{
	}
}
