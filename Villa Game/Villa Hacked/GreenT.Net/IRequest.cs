using System;

namespace GreenT.Net;

public interface IRequest<T> where T : Response
{
	IObservable<T> Request(params string[] args);
}
