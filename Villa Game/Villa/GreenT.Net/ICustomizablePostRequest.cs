using System;
using System.Collections.Generic;

namespace GreenT.Net;

public interface ICustomizablePostRequest<TResponse, TValue>
{
	IObservable<TResponse> Post(TValue fields, params object[] objects);
}
public interface ICustomizablePostRequest<TResponse> : ICustomizablePostRequest<TResponse, IDictionary<string, string>>
{
}
