using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Games.Coresdk.Unity;

public class APIUtil
{
	public static string GetQueryString(Dictionary<string, string> query)
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, string> item2 in query)
		{
			string item = item2.Key + "=" + item2.Value;
			list.Add(item);
		}
		return string.Join("&", list.ToArray());
	}

	internal static void Get(string domain, string endpoint, Dictionary<string, string> queryStringMap, string authorization, Action<RawResponse> callback, MonoBehaviour monoBehaviour)
	{
		string url = domain + endpoint;
		monoBehaviour.StartCoroutine(RequestAPI(url, queryStringMap, authorization, null, callback));
	}

	internal static void Post(string domain, string endpoint, Dictionary<string, string> formMap, string authorization, Action<RawResponse> callback, MonoBehaviour monoBehaviour)
	{
		string url = domain + endpoint;
		string queryString = GetQueryString(formMap);
		monoBehaviour.StartCoroutine(RequestAPI(url, null, authorization, queryString, callback));
	}

	internal static IEnumerator RequestAPI(string url, Dictionary<string, string> queryStringMap, string authorization, string body, Action<RawResponse> callback)
	{
		if (queryStringMap != null)
		{
			string queryString = GetQueryString(queryStringMap);
			url = url + "?" + queryString;
		}
		UnityWebRequest webRequest = UnityWebRequest.Get(url);
		try
		{
			webRequest.method = (string.IsNullOrEmpty(body) ? "GET" : "POST");
			if (!string.IsNullOrEmpty(authorization))
			{
				webRequest.SetRequestHeader("Authorization", authorization);
			}
			if (!string.IsNullOrEmpty(body))
			{
				webRequest.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
				byte[] bytes = Encoding.UTF8.GetBytes(body);
				webRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(bytes);
			}
			if (Application.isEditor || Debug.isDebugBuild)
			{
				DumpRequest(webRequest, authorization, body);
			}
			yield return webRequest.SendWebRequest();
			if (Application.isEditor || Debug.isDebugBuild)
			{
				DumpResponse(webRequest);
			}
			int statusCode = (int)webRequest.responseCode;
			string text = webRequest.downloadHandler.text;
			string exception = string.Empty;
			if (webRequest.isNetworkError || webRequest.isHttpError)
			{
				exception = webRequest.error;
			}
			RawResponse obj = new RawResponse(statusCode, text, exception);
			callback(obj);
		}
		finally
		{
			((IDisposable)webRequest)?.Dispose();
		}
	}

	private static void DumpRequest(UnityWebRequest request, string authorization, string body)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendFormat("[{0}] {1}", request.method, request.url).AppendLine();
		if (!string.IsNullOrEmpty(authorization))
		{
			stringBuilder.AppendFormat("Authorization:{0}", authorization).AppendLine();
		}
		if (!string.IsNullOrEmpty(body))
		{
			stringBuilder.AppendFormat("Body:{0}", body);
		}
		Debug.Log(stringBuilder.ToString());
	}

	private static void DumpResponse(UnityWebRequest request)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendFormat("Response Status Code:{0}", request.responseCode).AppendLine();
		if (request.isNetworkError || request.isHttpError)
		{
			stringBuilder.AppendFormat("Error:{0}", request.error).AppendLine();
		}
		else
		{
			stringBuilder.AppendFormat("Data:{0}", request.downloadHandler.text);
		}
		Debug.Log(stringBuilder.ToString());
	}
}
