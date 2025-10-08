using System;

namespace Games.Coresdk.Unity;

public class LogoutResult
{
	public Exception Exception { get; private set; }

	public LogoutResult()
	{
	}

	public LogoutResult(Exception exception)
	{
		Exception = exception;
	}
}
