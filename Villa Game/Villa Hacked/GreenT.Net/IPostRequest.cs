using System;
using System.Collections.Generic;

namespace GreenT.Net;

public interface IPostRequest<TResponse, TValue>
{
	IObservable<TResponse> Post(TValue fields);
}
public interface IPostRequest<TResponse> : IPostRequest<TResponse, IDictionary<string, string>>
{
}
