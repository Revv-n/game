using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class ServerPostRequest : MonoBehaviour
{
	public const int RequestTimeout = 15;

	public static ServerPostRequest Instance { get; private set; }

	private void Start()
	{
		Init();
	}

	public static void Init()
	{
		if (!(Instance != null))
		{
			Instance = UnityEngine.Object.FindObjectOfType<ServerPostRequest>();
			if (!(Instance != null))
			{
				Instance = new GameObject("ServerPostRequest").AddComponent<ServerPostRequest>();
			}
		}
	}

	public static void Post<TInput>(TInput input_json, string url, Action<bool> on_complete = null)
	{
		Init();
		Instance.StartCoroutine(Instance.PostRoutine(input_json, url, on_complete));
	}

	public static void Request<TOutput>(string url, Action<TOutput, bool> on_complete)
	{
		Init();
		Instance.StartCoroutine(Instance.RequestRoutine(url, on_complete));
	}

	public static void PostRequest<TInput, TOutput>(TInput input_json, string url, Action<TOutput, bool, string> on_complete)
	{
		Init();
		Instance.StartCoroutine(Instance.PostRequestRoutine(input_json, url, on_complete));
	}

	public static void PostRequest<TInput>(TInput input_json, string url, Action<string, bool, string> on_complete)
	{
		Init();
		Instance.StartCoroutine(Instance.PostRequestRoutine(input_json, url, on_complete));
	}

	public IEnumerator RequestRoutine<TOutput>(string url, Action<TOutput, bool> on_complete)
	{
		Stopwatch t = new Stopwatch();
		t.Start();
		UnityWebRequest request = UnityWebRequest.Get(url);
		request.timeout = 15;
		yield return request.SendWebRequest();
		if (request.isNetworkError || request.isHttpError)
		{
			UnityEngine.Debug.LogErrorFormat("Request is failed by url [{0}]!", request.error);
			on_complete?.Invoke(default(TOutput), arg2: false);
		}
		else
		{
			TOutput arg = JsonUtility.FromJson<TOutput>(request.downloadHandler.text);
			on_complete?.Invoke(arg, arg2: true);
			request.Dispose();
			t.Stop();
		}
	}

	public IEnumerator PostRoutine<TInput>(TInput input_json, string url, Action<bool> on_complete)
	{
		Stopwatch t = new Stopwatch();
		t.Start();
		string text = JsonUtility.ToJson(input_json);
		byte[] data = new byte[0];
		if (text != null)
		{
			data = Encoding.UTF8.GetBytes(text);
		}
		UploadHandlerRaw uploadHandlerRaw = new UploadHandlerRaw(data);
		uploadHandlerRaw.contentType = "application/json";
		UnityWebRequest request = UnityWebRequest.PostWwwForm(url, "POST");
		request.timeout = 15;
		request.uploadHandler = uploadHandlerRaw;
		yield return request.SendWebRequest();
		if (request.isNetworkError || request.isHttpError)
		{
			on_complete?.Invoke(obj: false);
		}
		else
		{
			on_complete?.Invoke(obj: true);
		}
		request.Dispose();
		t.Stop();
	}

	public IEnumerator PostRequestRoutine<TInput, TOutput>(TInput json_object, string url, Action<TOutput, bool, string> on_complete = null)
	{
		Stopwatch t = new Stopwatch();
		t.Start();
		string text = JsonUtility.ToJson(json_object);
		UploadHandlerRaw uploadHandlerRaw = new UploadHandlerRaw((text != null) ? Encoding.UTF8.GetBytes(text) : new byte[0]);
		uploadHandlerRaw.contentType = "application/json";
		UnityWebRequest request = UnityWebRequest.PostWwwForm(url, "POST");
		request.timeout = 15;
		request.uploadHandler = uploadHandlerRaw;
		yield return request.SendWebRequest();
		if (request.isNetworkError || request.isHttpError)
		{
			UnityEngine.Debug.LogErrorFormat("Request is failed by url [{0}]!", request.error);
			on_complete?.Invoke(default(TOutput), arg2: false, request.error);
		}
		else
		{
			TOutput arg = JsonUtility.FromJson<TOutput>(request.downloadHandler.text);
			on_complete?.Invoke(arg, arg2: true, string.Empty);
		}
		request.Dispose();
		t.Stop();
	}

	public IEnumerator PostRequestRoutine<TInput>(TInput json_object, string url, Action<string, bool, string> on_complete = null)
	{
		Stopwatch t = new Stopwatch();
		t.Start();
		string text = JsonUtility.ToJson(json_object);
		byte[] data = new byte[0];
		if (text != null)
		{
			data = Encoding.UTF8.GetBytes(text);
		}
		UploadHandlerRaw uploadHandlerRaw = new UploadHandlerRaw(data);
		uploadHandlerRaw.contentType = "application/json";
		UnityWebRequest request = UnityWebRequest.PostWwwForm(url, "POST");
		request.timeout = 15;
		request.uploadHandler = uploadHandlerRaw;
		yield return request.SendWebRequest();
		if (request.isNetworkError || request.isHttpError)
		{
			UnityEngine.Debug.LogErrorFormat("Request is failed by url [{0}]!", request.error);
			on_complete?.Invoke(null, arg2: false, request.error);
		}
		else
		{
			UnityEngine.Debug.Log("Got simple request");
			on_complete?.Invoke(request.downloadHandler.text, arg2: true, string.Empty);
		}
		request.Dispose();
		t.Stop();
	}
}
