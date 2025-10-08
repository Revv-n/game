using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace SM.Web.Management;

public class WebManager : MonoBehaviour
{
	private static WebManager instance;

	public const int DefaultRequestTimeout = 15;

	public static float InternetStatementCheckDelay = 1f;

	public static readonly TimeSpan DefaultInfoObsolescenceTime = TimeSpan.FromMinutes(5.0);

	private static List<WebRequestInfo> m_ActiveRequests = new List<WebRequestInfo>();

	public static WebManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = UnityEngine.Object.FindObjectOfType<WebManager>();
				if (instance == null)
				{
					instance = new GameObject("WebManager").AddComponent<WebManager>();
				}
			}
			return instance;
		}
	}

	private static bool observationsInProgress { get; set; } = false;


	public static NetworkReachability emulatedNetworkReachability
	{
		get
		{
			return (NetworkReachability)PlayerPrefs.GetInt("emulated_network_reachability", (int)Application.internetReachability);
		}
		set
		{
			PlayerPrefs.SetInt("emulated_network_reachability", (int)value);
			PlayerPrefs.Save();
		}
	}

	public static bool networkReachabilityEmulation
	{
		get
		{
			return PlayerPrefs.GetInt("network_reachability_emulation", 0) == 1;
		}
		set
		{
			PlayerPrefs.SetInt("network_reachability_emulation", value ? 1 : 0);
			PlayerPrefs.Save();
		}
	}

	public static NetworkReachability internetReachability
	{
		get
		{
			if (!networkReachabilityEmulation)
			{
				return Application.internetReachability;
			}
			return emulatedNetworkReachability;
		}
	}

	private static WebTimeNow dateTimeNow { get; set; } = () => DateTime.Now;


	private static event ConnectionStatementChanged connectionStatementChanged;

	public static event ConnectionStatementChanged ConnectionObserver
	{
		add
		{
			connectionStatementChanged += value;
			Instance.StartCoroutine(Instance.InternetConnectionStatementObservation());
		}
		remove
		{
			connectionStatementChanged -= value;
			if (observationsInProgress && WebManager.connectionStatementChanged != null)
			{
				Delegate[] invocationList = WebManager.connectionStatementChanged.GetInvocationList();
				if (invocationList != null && invocationList.Any())
				{
					return;
				}
			}
			observationsInProgress = false;
		}
	}

	public IEnumerator InternetConnectionStatementObservation()
	{
		if (observationsInProgress)
		{
			yield break;
		}
		observationsInProgress = true;
		WaitForSeconds waiter = new WaitForSeconds(InternetStatementCheckDelay);
		NetworkReachability currentStatement = internetReachability;
		while (observationsInProgress)
		{
			yield return waiter;
			if (currentStatement != internetReachability)
			{
				ConnectionStatementChanged obj = WebManager.connectionStatementChanged;
				if (obj != null)
				{
					NetworkReachability current;
					currentStatement = (current = internetReachability);
					obj(current);
				}
			}
		}
	}

	public static bool GetAcitveRequest(string url, out WebRequestInfo info)
	{
		if (m_ActiveRequests == null)
		{
			m_ActiveRequests = new List<WebRequestInfo>();
		}
		info = m_ActiveRequests.FirstOrDefault((WebRequestInfo r) => r.Url == url);
		return info != null;
	}

	public void SetDateTimeNowGetter(WebTimeNow webTimeNow)
	{
		dateTimeNow = webTimeNow;
	}

	public static void Post<TInput>(TInput content, string url, Action<bool> onComplete = null)
	{
		Instance.StartCoroutine(PostRoutine(content, url, onComplete));
	}

	public static void Request<TOutput>(string url, WebRequestResultCallback<TOutput> onComplete)
	{
		Instance.StartCoroutine(RequestRoutine(url, onComplete));
	}

	public static void PostRequest<TOutput>(object content, string url, WebRequestResultCallback<TOutput> onComplete)
	{
		Instance.StartCoroutine(PostRequestRoutine(content, url, onComplete));
	}

	public static void PostRequest(object content, Type outputType, string url, WebRequestResultCallback onComplete)
	{
		Instance.StartCoroutine(PostRequestRoutine(content, outputType, url, onComplete));
	}

	public static void PostRequest<TInput, TOutput>(TInput input_json, string url, Action<WebRequestInfo> getReqInfo, WebRequestResultCallback<TOutput> on_complete)
	{
		Instance.StartCoroutine(PostRequestRoutine(input_json, url, on_complete));
	}

	public static IEnumerator RequestRoutine<TOutput>(string url, WebRequestResultCallback<TOutput> on_complete)
	{
		return RequestRoutine(url, null, on_complete);
	}

	public static IEnumerator RequestRoutine<TOutput>(string url, WebRequestProgressCallback onProgress, WebRequestResultCallback<TOutput> onComplete)
	{
		Stopwatch t = new Stopwatch();
		t.Start();
		UnityWebRequest request = UnityWebRequest.Get(url);
		request.timeout = 15;
		WebRequestInfo requestInfo = new WebRequestInfo(request, dateTimeNow());
		m_ActiveRequests.Add(requestInfo);
		UnityWebRequestAsyncOperation requestRoutine = request.SendWebRequest();
		do
		{
			onProgress?.Invoke(((AsyncOperation)(object)requestRoutine).progress);
			requestInfo.InvokeOnProgress(((AsyncOperation)(object)requestRoutine).progress);
			yield return null;
		}
		while (!((AsyncOperation)(object)requestRoutine).isDone);
		onProgress?.Invoke(1f);
		requestInfo.InvokeOnProgress(1f);
		if (request.isNetworkError || request.isHttpError)
		{
			UnityEngine.Debug.LogErrorFormat("SERVER MANAGER ERROR: Request is failed by url [{0}]! by url [{1}]", request.error, url);
			onComplete?.Invoke(default(TOutput), result: false, request.error);
			requestInfo.InvokeOnCompleted(result: false);
		}
		else
		{
			TOutput content = JsonConvert.DeserializeObject<TOutput>(request.downloadHandler.text);
			onComplete?.Invoke(content, result: true, string.Empty);
			requestInfo.InvokeOnCompleted(result: true);
		}
		m_ActiveRequests.Remove(requestInfo);
		t.Stop();
	}

	public static IEnumerator PostRoutine<TInput>(TInput content, string url, Action<bool> onComplete)
	{
		Stopwatch t = new Stopwatch();
		t.Start();
		string text = JsonUtility.ToJson((object)content);
		byte[] array = new byte[0];
		if (text != null)
		{
			array = Encoding.UTF8.GetBytes(text);
		}
		UploadHandlerRaw val = new UploadHandlerRaw(array);
		((UploadHandler)val).contentType = "application/json";
		UnityWebRequest request = UnityWebRequest.PostWwwForm(url, "POST");
		request.timeout = 15;
		request.uploadHandler = (UploadHandler)(object)val;
		WebRequestInfo req = new WebRequestInfo(request, dateTimeNow());
		m_ActiveRequests.Add(req);
		yield return request.SendWebRequest();
		if (request.isNetworkError || request.isHttpError)
		{
			UnityEngine.Debug.LogFormat("SERVER MANAGER LOG: Request is failed by url [{0}]! Error: [{1}]", url, request.error);
			onComplete?.Invoke(obj: false);
			req.InvokeOnCompleted(result: false);
		}
		else
		{
			onComplete?.Invoke(obj: true);
			req.InvokeOnCompleted(result: true);
		}
		m_ActiveRequests.Remove(req);
		t.Stop();
	}

	public static IEnumerator PostRequestRoutine<TInput, TOutput>(TInput content, string url, WebRequestResultCallback<TOutput> onComplete)
	{
		Stopwatch t = new Stopwatch();
		t.Start();
		string text = JsonUtility.ToJson((object)content);
		byte[] array = new byte[0];
		if (text != null)
		{
			array = Encoding.UTF8.GetBytes(text);
		}
		UploadHandlerRaw val = new UploadHandlerRaw(array);
		((UploadHandler)val).contentType = "application/json";
		UnityWebRequest request = UnityWebRequest.PostWwwForm(url, "POST");
		request.timeout = 15;
		request.uploadHandler = (UploadHandler)(object)val;
		WebRequestInfo req = new WebRequestInfo(request, dateTimeNow());
		m_ActiveRequests.Add(req);
		yield return request.SendWebRequest();
		if (request.isNetworkError || request.isHttpError)
		{
			UnityEngine.Debug.LogErrorFormat("SERVER MANAGER ERROR: Request is failed by url [{0}]! Error: [{1}]", url, request.error);
			onComplete?.Invoke(default(TOutput), result: false, request.error);
			req.InvokeOnCompleted(result: false);
		}
		else
		{
			TOutput val2 = JsonConvert.DeserializeObject<TOutput>(request.downloadHandler.text);
			TOutput val3 = val2;
			UnityEngine.Debug.Log("json " + val3);
			onComplete?.Invoke(val2, result: true, string.Empty);
			req.InvokeOnCompleted(result: true);
		}
		m_ActiveRequests.Remove(req);
		t.Stop();
	}

	public static IEnumerator PostRequestRoutine(object content, Type outputType, string url, WebRequestResultCallback onComplete)
	{
		Stopwatch t = new Stopwatch();
		t.Start();
		string text = JsonUtility.ToJson(content);
		byte[] array = new byte[0];
		if (text != null)
		{
			array = Encoding.UTF8.GetBytes(text);
		}
		UploadHandlerRaw val = new UploadHandlerRaw(array);
		((UploadHandler)val).contentType = "application/json";
		UnityWebRequest request = UnityWebRequest.PostWwwForm(url, "POST");
		request.timeout = 15;
		request.uploadHandler = (UploadHandler)(object)val;
		WebRequestInfo req = new WebRequestInfo(request, dateTimeNow());
		m_ActiveRequests.Add(req);
		yield return request.SendWebRequest();
		if (request.isNetworkError || request.isHttpError)
		{
			UnityEngine.Debug.LogErrorFormat("SERVER MANAGER ERROR: Request is failed by url [{0}]! Error: [{1}]", url, request.error);
			onComplete?.Invoke(null, null, result: false, request.error);
			req.InvokeOnCompleted(result: false);
		}
		else
		{
			object content2 = JsonUtility.FromJson(request.downloadHandler.text, outputType);
			onComplete?.Invoke(content2, outputType, result: true, string.Empty);
			req.InvokeOnCompleted(result: true);
		}
		m_ActiveRequests.Remove(req);
		t.Stop();
	}

	public static string Combine(string url, string api)
	{
		return string.Join("/", url, api);
	}
}
