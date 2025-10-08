using System;
using UnityEngine.Networking;

namespace SM.Web.Management;

public class WebRequestInfo
{
	public delegate void RequestResultCallback(bool result);

	protected Action abortRelatedWebRequest;

	public string Url { get; private set; }

	public DateTime RequestExpirationDate { get; private set; }

	public float Progress { get; private set; }

	public event WebRequestProgressCallback OnProgress;

	public event RequestResultCallback OnRequestCompleted;

	public WebRequestInfo(string url, int requestTimeout, Action abortRelatedWebRequest, DateTime nowTime)
	{
		Url = url;
		RequestExpirationDate = nowTime.AddSeconds(requestTimeout);
		this.abortRelatedWebRequest = abortRelatedWebRequest;
	}

	public WebRequestInfo(UnityWebRequest request, DateTime nowTime)
		: this(request.url, 15, (Action)request.Abort, nowTime)
	{
	}

	public void InvokeOnCompleted(bool result)
	{
		this.OnRequestCompleted?.Invoke(result);
	}

	public void Abort()
	{
		abortRelatedWebRequest?.Invoke();
	}

	public void InvokeOnProgress(float progress)
	{
		Progress = progress;
		this.OnProgress?.Invoke(progress);
	}
}
