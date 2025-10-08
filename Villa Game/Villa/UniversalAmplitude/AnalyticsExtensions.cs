using UnityEngine.Networking;

namespace UniversalAmplitude;

public static class AnalyticsExtensions
{
	public static UnityWebRequestAwaiter GetAwaiter(this UnityWebRequestAsyncOperation asyncOp)
	{
		return new UnityWebRequestAwaiter(asyncOp);
	}
}
