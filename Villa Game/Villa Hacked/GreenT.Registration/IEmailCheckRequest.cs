using System;
using GreenT.Net;

namespace GreenT.Registration;

public interface IEmailCheckRequest
{
	IObservable<Response> Check(string mail);
}
