using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;

namespace GreenT.AssetBundles.Communication;

public abstract class BaseFileRequest<TResponse> where TResponse : BaseResponse
{
	protected UnityWebRequest unityWebRequest;

	public TResponse Response { get; protected set; }

	public Exception Error { get; protected set; }

	public virtual bool IsAborted { get; protected set; }

	public virtual bool IsExitedError
	{
		get
		{
			if (Response.retryCount < GetRetryCount())
			{
				return IsAborted;
			}
			return true;
		}
	}

	public virtual bool HasError => Error != null;

	protected void Initialize()
	{
		unityWebRequest = null;
		Error = null;
		Response.isCached = false;
		Response.retryCount = 0u;
		IsAborted = false;
	}

	public void Abort()
	{
		UnityWebRequest obj = unityWebRequest;
		if (obj != null)
		{
			obj.Abort();
		}
		IsAborted = true;
	}

	public abstract IEnumerator Send();

	protected abstract uint GetRetryCount();

	protected IEnumerator DownloadFile(Func<UnityWebRequest> unityWebRequestFactory, Action<UnityWebRequest> onSuccess)
	{
		Error = null;
		if (!IsExitedError)
		{
			UnityWebRequest webRequest = unityWebRequestFactory();
			yield return SendWebRequest(webRequest, onSuccess, () => DownloadFile(unityWebRequestFactory, onSuccess));
		}
	}

	protected IEnumerator SendWebRequest(UnityWebRequest webRequest, Action<UnityWebRequest> onSuccess, Func<IEnumerator> onError)
	{
		if (!HasError)
		{
			UnityWebRequest val = (unityWebRequest = webRequest);
			try
			{
				yield return unityWebRequest.SendWebRequest();
				if (unityWebRequest.isNetworkError || unityWebRequest.isHttpError)
				{
					Error = new UnityWebRequestException(unityWebRequest);
					HandleException("It's a network or http error", Error);
				}
				if (!HasError)
				{
					onSuccess(unityWebRequest);
				}
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		if (HasError)
		{
			yield return onError();
		}
	}

	protected Uri CreateUri(string url)
	{
		Uri result = null;
		try
		{
			result = new Uri(url);
		}
		catch (Exception exception)
		{
			string message = "Wrong URI format " + url + "\n";
			HandleException(message, exception);
		}
		return result;
	}

	protected virtual void HandleException(string message, Exception exception = null)
	{
		Error = ((exception == null) ? new Exception(message) : new Exception(message, exception));
		Response.retryCount++;
		Error.LogException();
	}
}
