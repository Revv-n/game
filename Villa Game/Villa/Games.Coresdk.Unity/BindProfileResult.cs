using System;

namespace Games.Coresdk.Unity;

public class BindProfileResult
{
	public Exception Exception { get; private set; }

	public ProfileResult.ProfileData Data { get; private set; }

	public bool IsSuccess { get; private set; }

	public static BindProfileResult Parse(ProfileResult profileResult, bool bindResult)
	{
		return new BindProfileResult
		{
			Data = profileResult.Data,
			Exception = profileResult.Exception,
			IsSuccess = (bindResult && profileResult.IsSuccess)
		};
	}
}
