using System;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Cysharp.Threading.Tasks;

public class UnityWebRequestException : Exception
{
	private string msg;

	public UnityWebRequest UnityWebRequest { get; }

	public Result Result { get; }

	public string Error { get; }

	public string Text { get; }

	public long ResponseCode { get; }

	public Dictionary<string, string> ResponseHeaders { get; }

	public override string Message
	{
		get
		{
			if (msg == null)
			{
				msg = Error;
				if (UnityWebRequest != null && !string.IsNullOrEmpty(UnityWebRequest.url))
				{
					msg = "Url: " + UnityWebRequest.url + "\n" + msg;
				}
				if (Text != null)
				{
					msg = msg + Environment.NewLine + Text;
				}
				msg = msg + $" ResponseCode = {ResponseCode} " + "\n";
			}
			return msg;
		}
	}

	public UnityWebRequestException(UnityWebRequest unityWebRequest)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		UnityWebRequest = unityWebRequest;
		Result = unityWebRequest.result;
		Error = unityWebRequest.error;
		ResponseCode = unityWebRequest.responseCode;
		if (UnityWebRequest.downloadHandler != null)
		{
			DownloadHandler downloadHandler = unityWebRequest.downloadHandler;
			DownloadHandlerBuffer val = (DownloadHandlerBuffer)(object)((downloadHandler is DownloadHandlerBuffer) ? downloadHandler : null);
			if (val != null)
			{
				Text = ((DownloadHandler)val).text;
			}
		}
		ResponseHeaders = unityWebRequest.GetResponseHeaders();
	}
}
