using System;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Cysharp.Threading.Tasks;

public class UnityWebRequestException : Exception
{
	private string msg;

	public UnityWebRequest UnityWebRequest { get; }

	public UnityWebRequest.Result Result { get; }

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
		UnityWebRequest = unityWebRequest;
		Result = unityWebRequest.result;
		Error = unityWebRequest.error;
		ResponseCode = unityWebRequest.responseCode;
		if (UnityWebRequest.downloadHandler != null && unityWebRequest.downloadHandler is DownloadHandlerBuffer downloadHandlerBuffer)
		{
			Text = downloadHandlerBuffer.text;
		}
		ResponseHeaders = unityWebRequest.GetResponseHeaders();
	}
}
