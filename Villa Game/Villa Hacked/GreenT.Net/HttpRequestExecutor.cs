using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using StripClub.Utility;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;

namespace GreenT.Net;

public static class HttpRequestExecutor
{
	private static Dictionary<Uri, string> jsonQueryCache = new Dictionary<Uri, string>();

	private static Dictionary<Uri, Queue<IObserver<string>>> observersWaitQueue = new Dictionary<Uri, Queue<IObserver<string>>>();

	public static IObservable<string> GetRequest(Uri uri, bool cached = false, IEnumerable<KeyValuePair<string, string>> headers = null)
	{
		return GetRequest(uri.AbsoluteUri, cached, headers);
	}

	public static IObservable<string> GetRequest(string url, bool cached = false, IEnumerable<KeyValuePair<string, string>> headers = null)
	{
		return Observable.FromCoroutine<string>((Func<IObserver<string>, IEnumerator>)((IObserver<string> observer) => GetRequestFromUrlRoutine(url, observer, cached, headers)));
	}

	public static IObservable<T> GetRequest<T>(string requestUrl, bool cached = false, IEnumerable<KeyValuePair<string, string>> headers = null)
	{
		return Observable.Catch<T, Exception>(Observable.Select<string, T>(GetRequest(requestUrl, cached, headers), (Func<string, T>)delegate(string _value)
		{
			try
			{
				return JsonConvert.DeserializeObject<T>(_value);
			}
			catch (Exception innerException)
			{
				throw new ArgumentException("Can't parse as " + typeof(T).FullName + " from string:\n" + _value, "json", innerException);
			}
		}), (Func<Exception, IObservable<T>>)delegate(Exception ex)
		{
			throw ex.SendException("Can't get " + typeof(T).FullName + " by url: " + requestUrl);
		});
	}

	public static IObservable<IEnumerable<T>> GetCollectionFromRequest<T>(string requestUrl, bool cached = false, IEnumerable<KeyValuePair<string, string>> headers = null)
	{
		return Observable.Catch<T[], Exception>(Observable.Select<string, T[]>(GetRequest(requestUrl, cached, headers), (Func<string, T[]>)JsonHelper.AsArray<T>), (Func<Exception, IObservable<T[]>>)delegate(Exception ex)
		{
			throw ex.SendException("Can't get collection of " + typeof(T).FullName + " by url " + requestUrl);
		});
	}

	public static IEnumerator GetRequestFromUrlRoutine(string url, IObserver<string> observer, bool cached = false, IEnumerable<KeyValuePair<string, string>> headers = null)
	{
		if (string.IsNullOrEmpty(url))
		{
			throw new ArgumentNullException("url is empty");
		}
		Uri uri = new Uri(url);
		if (cached)
		{
			if (jsonQueryCache.ContainsKey(uri))
			{
				observer.OnNext(jsonQueryCache[uri]);
				observer.OnCompleted();
				yield break;
			}
			if (observersWaitQueue.TryGetValue(uri, out var value))
			{
				value.Enqueue(observer);
				yield break;
			}
			observersWaitQueue[uri] = new Queue<IObserver<string>>();
		}
		UnityWebRequest val;
		try
		{
			val = UnityWebRequest.Get(uri);
			if (headers != null)
			{
				foreach (KeyValuePair<string, string> header in headers)
				{
					val.SetRequestHeader(header.Key, header.Value);
				}
			}
		}
		catch (Exception innerException)
		{
			observer.OnError(new Exception("Incorrect URI:" + url + "\n", innerException));
			yield break;
		}
		yield return Send(val, observer);
	}

	public static IObservable<string> PostJSONRequest(string url, string jsonStr, IDictionary<string, string> headers = null)
	{
		return Observable.FromCoroutine<string>((Func<IObserver<string>, IEnumerator>)((IObserver<string> observer) => PostJSONRequestFromUrlRoutine(url, jsonStr, observer, headers)));
	}

	public static IEnumerator PostJSONRequestFromUrlRoutine(string url, string jsonStr, IObserver<string> observer, IDictionary<string, string> headers)
	{
		if (string.IsNullOrEmpty(url))
		{
			throw new ArgumentNullException("url is empty");
		}
		UnityWebRequest request = new UnityWebRequest(url);
		try
		{
			byte[] bytes = Encoding.UTF8.GetBytes(jsonStr);
			request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bytes);
			request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
			request.method = "POST";
			request.SetRequestHeader("Content-Type", "application/json");
			if (headers != null)
			{
				foreach (KeyValuePair<string, string> header in headers)
				{
					request.SetRequestHeader(header.Key, header.Value);
				}
			}
			yield return Send(request, observer);
		}
		finally
		{
			((IDisposable)request)?.Dispose();
		}
	}

	public static IObservable<T> PostRequest<T>(string url, IDictionary<string, string> fields)
	{
		return Observable.Select<string, T>(PostRequest(url, fields), (Func<string, T>)JsonConvert.DeserializeObject<T>);
	}

	public static IObservable<string> PostRequest(string url, IDictionary<string, string> fields)
	{
		return Observable.FromCoroutine<string>((Func<IObserver<string>, IEnumerator>)((IObserver<string> observer) => PostRequestFromUrlRoutine(url, fields, observer)));
	}

	public static IEnumerator PostRequestFromUrlRoutine(string url, IDictionary<string, string> fields, IObserver<string> observer)
	{
		if (string.IsNullOrEmpty(url))
		{
			throw new ArgumentNullException("url is empty");
		}
		WWWForm val = new WWWForm();
		foreach (KeyValuePair<string, string> field in fields)
		{
			val.AddField(field.Key, field.Value);
		}
		val.headers["Accept"] = "application/json";
		UnityWebRequest request = UnityWebRequest.Post(url, val);
		try
		{
			yield return Send(request, observer);
		}
		finally
		{
			((IDisposable)request)?.Dispose();
		}
	}

	private static IEnumerator Send(UnityWebRequest request, IObserver<string> observer)
	{
		yield return request.SendWebRequest();
		if (request.isNetworkError || request.isHttpError)
		{
			DownloadHandler downloadHandler = request.downloadHandler;
			if (downloadHandler == null)
			{
				Debug.LogError($"HttpRequestExecutor: request.downloadHandler is null, request is {request}");
			}
			else if (downloadHandler.text == null)
			{
				Debug.LogError($"HttpRequestExecutor: request.downloadHandler.text is null, request.downloadHandler is {downloadHandler}");
			}
			else
			{
				Debug.LogError($"HttpRequestExecutor: error with request: {request.url}, request.downloadHandler: {downloadHandler}, request.downloadHandler.text: {downloadHandler.text}");
			}
			UnityWebRequestException error = new UnityWebRequestException(request);
			observer.OnError(error);
			yield break;
		}
		string text = request.downloadHandler.text;
		jsonQueryCache[request.uri] = text;
		try
		{
			observer.OnNext(text);
			observer.OnCompleted();
			observersWaitQueue.ToArray();
			if (observersWaitQueue.TryGetValue(request.uri, out var value))
			{
				while (value.Any())
				{
					IObserver<string> observer2 = value.Dequeue();
					observer2.OnNext(text);
					observer2.OnCompleted();
				}
				observersWaitQueue.Remove(request.uri);
			}
		}
		catch (Exception innerException)
		{
			throw new Exception("Exception on processing response from: <color=#C20900>" + request.url + "</color> ", innerException);
		}
	}

	public static IObservable<Sprite> GetSprite(string url)
	{
		return Observable.Catch<Sprite, Exception>(Observable.FromCoroutine<Sprite>((Func<IObserver<Sprite>, IEnumerator>)((IObserver<Sprite> observer) => GetSprite(url, observer))), (Func<Exception, IObservable<Sprite>>)delegate(Exception ex)
		{
			throw ex;
		});
	}

	public static IEnumerator GetSprite(string url, IObserver<Sprite> observer)
	{
		UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
		yield return request.SendWebRequest();
		if (request.isNetworkError)
		{
			observer.OnError(new Exception("Connection error:" + request.error));
			yield break;
		}
		Texture2D content = DownloadHandlerTexture.GetContent(request);
		Sprite sprite = Sprite.Create(content, new Rect(0f, 0f, content.width, content.height), new Vector2(0.5f, 0.5f));
		sprite.name = Path.GetFileName(url);
		observer.OnNext(sprite);
		observer.OnCompleted();
	}
}
