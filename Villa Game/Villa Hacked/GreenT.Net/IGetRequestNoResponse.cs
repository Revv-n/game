using System;

namespace GreenT.Net;

public interface IGetRequestNoResponse<T>
{
	IObservable<T> GetRequest(string requestUrl, params object[] args);
}
