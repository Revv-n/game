using System;

namespace GreenT.Net;

public interface IGetRequest<T> where T : Response
{
	IObservable<T> GetRequest(string requestUrl, params object[] args);
}
public interface IGetRequest
{
	IObservable<T> GetRequest<T>(string requestUrl, params object[] args) where T : Response;
}
