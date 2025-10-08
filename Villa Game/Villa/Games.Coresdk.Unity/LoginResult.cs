using System;

namespace Games.Coresdk.Unity;

public class LoginResult
{
	public string Token { get; private set; }

	public Exception Exception { get; private set; }

	public static LoginResult Parse(RawResponse rawResponse)
	{
		LoginResult loginResult = new LoginResult();
		if (rawResponse.Exception != null)
		{
			loginResult.Exception = rawResponse.Exception;
			return loginResult;
		}
		try
		{
			JSONNode jSONNode = JSON.Parse(rawResponse.Data);
			loginResult.Token = jSONNode["jwt"].Value;
			return loginResult;
		}
		catch (Exception exception)
		{
			loginResult.Exception = exception;
			return loginResult;
		}
	}
}
