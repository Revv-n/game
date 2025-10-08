using System.Collections.Generic;

namespace GreenT.Net;

public interface IObjectPostRequest<TResponse> : IPostRequest<TResponse, IDictionary<string, object>>
{
}
