using System;

namespace GreenT.Data;

public class SaverState
{
	public Exception Exception;

	public long CurrentUpdatedAt { get; set; }

	public string CurrentDataString { get; set; }

	public bool HasError { get; private set; }

	public void SetData(UserDataMapper dataString)
	{
		CurrentUpdatedAt = dataString.UpdatedAt;
		CurrentDataString = dataString.Data;
	}

	public void HandleException(Exception exception)
	{
		HasError = true;
		Exception = exception;
	}
}
