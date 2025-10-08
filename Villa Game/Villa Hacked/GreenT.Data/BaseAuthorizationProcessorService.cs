using System;
using GreenT.Net;

namespace GreenT.Data;

public class BaseAuthorizationProcessorService
{
	private readonly SaverState _saverState;

	public BaseAuthorizationProcessorService(SaverState saverState)
	{
		_saverState = saverState;
	}

	protected virtual void OnAuthorize(Response<UserDataMapper> response)
	{
		if (response.Status != 0)
		{
			_saverState.HandleException(new Exception(GetType().Name + ": OnAuthorize has error: " + response.Error));
			_saverState.Exception.LogException();
		}
		else
		{
			_saverState.SetData(response.Data);
		}
	}

	protected void OnError(Exception exception)
	{
		_saverState.HandleException(exception);
		exception.LogException();
	}
}
